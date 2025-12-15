using CarpetBG.Application.DTOs.Additions;
using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Enums;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace CarpetBG.Infrastructure.Repositories;

public class OrderRepository(AppDbContext context, IDateTimeProvider dateTimeProvider) : IOrderRepository
{
    public async Task<(List<OrderDto> Items, int TotalCount)> GetFilteredAsync(OrderFilterDto filter, bool includeDeleted = false)
    {
        var query = context.Orders.Include(i => i.Items).AsNoTracking().AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(i => !i.IsDeleted);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTermNormalized = filter.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(i =>
                (i.User != null && i.User.PhoneNumber.ToLower().Contains(searchTermNormalized))
                || i.PickupAddress.DisplayAddress.Contains(searchTermNormalized));
        }

        var hasStatusFilter = filter.Statuses.Count > 0;

        if (hasStatusFilter)
        {
            query = query.Where(i => filter.Statuses.Contains(i.Status));
            DateTime? pickupDate = filter.PickupDate.HasValue ? dateTimeProvider.ToUtc(filter.PickupDate.Value.Date) : null;
            DateTime? nextPickupDay = pickupDate.HasValue ? pickupDate.Value.AddDays(1) : null;

            DateTime? deliveryDate = filter.DeliveryDate.HasValue ? dateTimeProvider.ToUtc(filter.DeliveryDate.Value.Date) : null;
            DateTime? nextDeliveryDay = deliveryDate.HasValue ? deliveryDate.Value.AddDays(1) : null;

            query = query.Where(i => (!pickupDate.HasValue && !deliveryDate.HasValue)
            || (pickupDate.HasValue && i.PickupDate >= pickupDate.Value && i.PickupDate < nextPickupDay)
            || (deliveryDate.HasValue && i.DeliveryDate >= deliveryDate && i.DeliveryDate < nextDeliveryDay));
        }

        if (!hasStatusFilter && filter.PickupDate.HasValue)
        {
            DateTime? pickupDate = filter.PickupDate.HasValue ? dateTimeProvider.ToUtc(filter.PickupDate.Value.Date) : null;
            DateTime? nextDay = pickupDate.HasValue ? pickupDate.Value.AddDays(1) : null;

            query = query.Where(i => i.PickupDate >= pickupDate && i.PickupDate < nextDay);
        }

        if (!hasStatusFilter && filter.DeliveryDate.HasValue)
        {
            DateTime? deliveryDate = filter.DeliveryDate.HasValue ? dateTimeProvider.ToUtc(filter.DeliveryDate.Value.Date) : null;
            DateTime? nextDay = deliveryDate.HasValue ? deliveryDate.Value.AddDays(1) : null;

            query = query.Where(i => i.DeliveryDate >= deliveryDate && i.DeliveryDate < nextDay);
        }

        var totalCount = await query.CountAsync();

        query = (filter.SortBy, filter.SortDirection) switch
        {
            (OrderSortBy.FullName, SortDirection.Desc) => query.OrderByDescending(o => o.User.FullName),
            (OrderSortBy.FullName, SortDirection.Asc) => query.OrderBy(o => o.User.FullName),

            (OrderSortBy.PhoneNumber, SortDirection.Desc) => query.OrderByDescending(o => o.User.PhoneNumber),
            (OrderSortBy.PhoneNumber, SortDirection.Asc) => query.OrderBy(o => o.User.PhoneNumber),

            (OrderSortBy.PickupDate, SortDirection.Desc) => query.OrderByDescending(o => o.PickupDate),
            (OrderSortBy.PickupDate, SortDirection.Asc) => query.OrderBy(o => o.PickupDate),

            (OrderSortBy.CreatedAt, SortDirection.Asc) => query.OrderBy(o => o.CreatedAt),
            _ => query.OrderByDescending(o => o.CreatedAt),
        };

        if (filter.PageSize > 0)
        {
            query = query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize);
        }

        var items = await query
            .Select(i => new OrderDto
            {
                Id = i.Id,
                IsExpress = i.Items.All(i => i.Additions.Any(a => a.NormalizedName == "express")),
                PickupAddress = i.PickupAddress.DisplayAddress,
                PickupDate = i.PickupDate,
                PickupAddressId = i.PickupAddressId,
                CustomerId = i.UserId,
                PhoneNumber = i.User.PhoneNumber,
                UserFullName = i.User.FullName,
                PickupTimeRange = i.PickupTimeRange,
                Status = i.Status,
                Note = i.Note,
                DeliveryAddress = i.DeliveryAddress.DisplayAddress,
                DeliveryAddressId = i.DeliveryAddressId,
                DeliveryDate = i.DeliveryDate,
                DeliveryTimeRange = i.DeliveryTimeRange,
                OrderItems = i.Items.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    Diagonal = oi.Diagonal,
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

    public async Task<Order?> GetByIdAsync(Guid id, bool includeDeleted = false, bool needTrackiing = false, bool includeItems = false)
    {
        var query = context.Orders
            .Include(o => o.User)
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
}
