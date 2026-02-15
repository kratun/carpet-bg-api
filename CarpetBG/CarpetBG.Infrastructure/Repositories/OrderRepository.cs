using CarpetBG.Application.Common;
using CarpetBG.Application.DTOs.Additions;
using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Enums;
using CarpetBG.Application.Helpers;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;
using CarpetBG.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace CarpetBG.Infrastructure.Repositories;

public class OrderRepository(AppDbContext context, IDateTimeProvider dateTimeProvider) : IOrderRepository
{
    private readonly List<OrderStatuses> _orderStatuses =
    [
        OrderStatuses.New,
        OrderStatuses.PendingPickup,
        OrderStatuses.PickupComplete,
        OrderStatuses.WashingInProgress,
        OrderStatuses.WashingComplete,
        OrderStatuses.PersonalDelivery,
        OrderStatuses.PendingDelivery,
        OrderStatuses.DeliveryComplete,
        OrderStatuses.Cancelled,
        OrderStatuses.Completed,
    ];

    public async Task<(List<OrderDto> Items, int TotalCount)> GetFilteredAsync(OrderFilterDto incomingFilter, bool includeDeleted = false)
    {
        var query = context.Orders.Include(i => i.Items).AsNoTracking().AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(i => !i.IsDeleted);
        }

        if (!string.IsNullOrWhiteSpace(incomingFilter.SearchTerm))
        {
            var searchTermNormalized = incomingFilter.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(i =>
                (i.Customer != null && i.Customer.PhoneNumber.ToLower().Contains(searchTermNormalized))
                || i.PickupAddress.DisplayAddress.Contains(searchTermNormalized));
        }

        var filter = incomingFilter.Filter;

        if (filter != null)
        {
            var targetStatuses = filter.Statuses;
            var unrestrictedStatuses =
                targetStatuses.Count == 0
                ? _orderStatuses
                : [.. targetStatuses
                        .Where(t => _orderStatuses
                                        .Contains(t.Status) && !t.Date.HasValue)
                                        .Select(t => t.Status)
                                        .Distinct()];

            query = query.Where(i => unrestrictedStatuses.Contains(i.Status));

            DateTime? pickupDate = filter!.PickupDate.HasValue ? dateTimeProvider.ToUtc(filter!.PickupDate.Value) : null;
            DateTime? nextPickupDay = pickupDate.HasValue ? pickupDate.Value.AddDays(1) : null;

            DateTime? deliveryDate = filter!.DeliveryDate.HasValue ? dateTimeProvider.ToUtc(filter!.DeliveryDate.Value) : null;
            DateTime? nextDeliveryDay = deliveryDate.HasValue ? deliveryDate.Value.AddDays(1) : null;

            query = query.Where(i =>
                   (!pickupDate.HasValue && !deliveryDate.HasValue)
                   || (deliveryDate.HasValue && i.DeliveryDate >= deliveryDate && i.DeliveryDate < nextDeliveryDay)
                   || (pickupDate.HasValue && i.PickupDate >= pickupDate.Value && i.PickupDate < nextPickupDay));
        }

        var totalCount = await query.CountAsync();

        query = SortBy(incomingFilter, query);

        var pageIndex = PaginationHelper.NormalizePageIndex(incomingFilter.PageIndex, incomingFilter.PageSize, totalCount);

        query = query.ApplyPagination(pageIndex, incomingFilter.PageSize);

        var items = await query
            .Select(i => new OrderDto
            {
                Id = i.Id,
                OrderNumber = i.OrderNumber,
                CreatedAt = i.CreatedAt,
                IsExpress = i.Items.All(i => i.Additions.Any(a => a.NormalizedName == "express")),
                PickupAddress = i.PickupAddress.DisplayAddress,
                PickupDate = i.PickupDate,
                PickupAddressId = i.PickupAddressId,
                CustomerId = i.CustomerId,
                PhoneNumber = i.Customer.PhoneNumber,
                CustomerFullName = i.Customer.FullName,
                PickupTimeRange = i.PickupTimeRange,
                Status = i.Status,
                Note = i.Note,
                DeliveryAddress = i.DeliveryAddress.DisplayAddress,
                DeliveryAddressId = i.DeliveryAddressId,
                DeliveryDate = i.DeliveryDate,
                DeliveryTimeRange = i.DeliveryTimeRange,
                OrderBy = i.OrderBy,
                OrderItems = i.Items.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    Height = oi.Height,
                    Note = oi.Note,
                    Price = oi.Price,
                    Width = oi.Width,
                    ProductId = oi.ProductId ?? Guid.Empty,
                    Status = oi.Status,
                    Additions = oi.Additions.Select(a => new AdditionDto
                    {
                        AdditionType = a.AdditionType,
                        Name = a.Name,
                        NormalizedName = a.NormalizedName,
                        Value = a.Value
                    }).ToList(),
                }).ToList(),
            })
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(List<OrderDto> Items, int TotalCount)> GetSetupLogisticDataAsync(OrderFilterDto incomingFilter, bool includeDeleted = false)
    {
        var query = context.Orders.Include(i => i.Items).AsNoTracking().AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(i => !i.IsDeleted);
        }

        if (!string.IsNullOrWhiteSpace(incomingFilter.SearchTerm))
        {
            var searchTermNormalized = incomingFilter.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(i =>
                (i.Customer != null && i.Customer.PhoneNumber.ToLower().Contains(searchTermNormalized))
                || i.PickupAddress.DisplayAddress.ToLowerInvariant().Contains(searchTermNormalized));
        }

        var hasStatusFilter = incomingFilter.Filter?.Statuses.Count > 0;

        if (hasStatusFilter)
        {
            var targetStatuses = incomingFilter.Filter!.Statuses;
            var unrestrictedStatuses = targetStatuses
                .Where(t =>
                    t.Status == OrderStatuses.New
                    || t.Status == OrderStatuses.WashingComplete
                    || (t.Status == OrderStatuses.PendingPickup && !t.Date.HasValue)
                    || (t.Status == OrderStatuses.PendingDelivery && !t.Date.HasValue))
                .Select(t => t.Status)
                .Distinct()
                .ToList();

            var pickupDates = targetStatuses
                .Where(t => t.Status == OrderStatuses.PendingPickup && t.Date.HasValue)
                .Select(t => dateTimeProvider.ToUtc(t.Date!.Value))
                .ToList();

            var deliveryDates = targetStatuses
                .Where(t => t.Status == OrderStatuses.PendingDelivery && t.Date.HasValue)
                .Select(t => dateTimeProvider.ToUtc(t.Date!.Value))
                .ToList();


            query = query.Where(i =>
                unrestrictedStatuses.Contains(i.Status)

                || (
                    i.Status == OrderStatuses.PendingPickup
                    && (!i.PickupDate.HasValue || pickupDates.Any(d => i.PickupDate.Value < d))
                )

                || (
                    i.Status == OrderStatuses.PendingDelivery
                    && (!i.DeliveryDate.HasValue || deliveryDates.Any(d => i.DeliveryDate.Value < d))
                )
            );

        }

        var totalCount = await query.CountAsync();
        query = SortBy(incomingFilter, query);

        var pageIndex = PaginationHelper.NormalizePageIndex(incomingFilter.PageIndex, incomingFilter.PageSize, totalCount);

        query = query.ApplyPagination(pageIndex, incomingFilter.PageSize);

        var items = await query
            .Select(i => new OrderDto
            {
                Id = i.Id,
                OrderNumber = i.OrderNumber,
                CreatedAt = i.CreatedAt,
                IsExpress = i.Items.All(i => i.Additions.Any(a => a.NormalizedName == "express")),
                PickupAddress = i.PickupAddress.DisplayAddress,
                PickupDate = i.PickupDate,
                PickupAddressId = i.PickupAddressId,
                CustomerId = i.CustomerId,
                PhoneNumber = i.Customer.PhoneNumber,
                CustomerFullName = i.Customer.FullName,
                PickupTimeRange = i.PickupTimeRange,
                Status = i.Status,
                Note = i.Note,
                DeliveryAddress = i.DeliveryAddress.DisplayAddress,
                DeliveryAddressId = i.DeliveryAddressId,
                DeliveryDate = i.DeliveryDate,
                DeliveryTimeRange = i.DeliveryTimeRange,
                OrderBy = i.OrderBy,
                OrderItems = i.Items.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    Height = oi.Height,
                    Note = oi.Note,
                    Price = oi.Price,
                    Width = oi.Width,
                    ProductId = oi.ProductId ?? Guid.Empty,
                    Additions = oi.Additions.Select(a => new AdditionDto
                    {
                        AdditionType = a.AdditionType,
                        Name = a.Name,
                        NormalizedName = a.NormalizedName,
                        Value = a.Value
                    }).ToList(),
                }).ToList(),
            })
            .ToListAsync();

        return (items, totalCount);
    }

    private static IQueryable<Order> SortBy(OrderFilterDto filter, IQueryable<Order> query)
    {
        query = (filter.SortBy, filter.SortDirection) switch
        {
            (OrderSortBy.FullName, SortDirection.Desc) => query.OrderByDescending(o => o.Customer.FullName),
            (OrderSortBy.FullName, SortDirection.Asc) => query.OrderBy(o => o.Customer.FullName),

            (OrderSortBy.PhoneNumber, SortDirection.Desc) => query.OrderByDescending(o => o.Customer.PhoneNumber),
            (OrderSortBy.PhoneNumber, SortDirection.Asc) => query.OrderBy(o => o.Customer.PhoneNumber),

            (OrderSortBy.PickupDate, SortDirection.Desc) => query.OrderByDescending(o => o.PickupDate),
            (OrderSortBy.PickupDate, SortDirection.Asc) => query.OrderBy(o => o.PickupDate),

            (OrderSortBy.CreatedAt, SortDirection.Asc) => query.OrderBy(o => o.CreatedAt),
            _ => query.OrderByDescending(o => o.CreatedAt),
        };
        return query;
    }

    public async Task<Order?> GetByIdAsync(Guid id, bool includeDeleted = false, bool needTrackiing = false, bool includeItems = false)
    {
        var query = context.Orders
            .Include(o => o.Customer)
            .Include(o => o.PickupAddress)
            .AsQueryable();

        if (includeItems)
        {
            query = query
                .Include(o => o.Items)
                .ThenInclude(i => i.Additions);
        }

        if (!needTrackiing)
        {
            query = query.AsNoTracking();
        }


        var entity = await query.FirstOrDefaultAsync(i => i.Id == id && (includeDeleted || !i.IsDeleted));

        return entity;
    }

    public async Task<List<Order>> GetByIdsAsync(IEnumerable<Guid> ids, bool includeDeleted = false, bool needTracking = false, bool includeItems = false)
    {
        var query = context.Orders
            .Include(o => o.Customer)
            .Include(o => o.PickupAddress)
            .AsQueryable();

        if (includeItems)
        {
            query = query
                .Include(o => o.Items)
                .ThenInclude(i => i.Additions);
        }

        if (!needTracking)
        {
            query = query.AsNoTracking();
        }

        if (!includeDeleted)
        {
            query = query
                .Where(i => !i.IsDeleted);
        }

        var entities = await query.Where(i => ids.Contains(i.Id)).ToListAsync();

        return entities;
    }

    public async Task<Order> AddAsync(Order order)
    {
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        return order;
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        context.Orders.Update(order);
        await context.SaveChangesAsync();

        return order;
    }

    public async Task<List<Order>> UpdateRangeAsync(List<Order> orders)
    {
        context.Orders.UpdateRange(orders);
        await context.SaveChangesAsync();

        return orders;
    }

    public async Task<Order?> GetByIdWithAllRelatedDataAsync(Guid id, bool needTrackiing = false, bool includeDeleted = false)
    {
        var query = context.Orders
            .Include(o => o.Customer)
            .Include(o => o.PickupAddress)
            .Include(o => o.DeliveryAddress)
            .Include(o => o.Items).ThenInclude(oi => oi.Additions)
            .Include(o => o.Items).ThenInclude(oi => oi.Product)
            .AsQueryable();

        if (!needTrackiing)
        {
            query = query.AsNoTracking();
        }


        var entity = await query.FirstOrDefaultAsync(i => i.Id == id && (includeDeleted || !i.IsDeleted));

        return entity;
    }
}
