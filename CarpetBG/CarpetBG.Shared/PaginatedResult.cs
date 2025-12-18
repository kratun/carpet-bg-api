using CarpetBG.Shared.Constants;

namespace CarpetBG.Shared;

public class PaginatedResult<T>(List<T> items, int totalCount, int pageNumber, int pageSize)
{
    public List<T> Items { get; } = items;
    public int TotalCount { get; } = totalCount;
    public int PageIndex { get; } = items.Count == 0 ? 0 : pageNumber;
    public int PageSize { get; } = pageSize;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageIndex > PaginationConstants.DefaultPageIndex;
    public bool HasNextPage => PageIndex < TotalPages;
}
