using CarpetBG.Shared.Constants;

namespace CarpetBG.Shared;

public class PaginatedResult<T>(IReadOnlyList<T> items, int totalCount, int pageIndex, int pageSize)
{
    public IReadOnlyList<T> Items { get; } = items;
    public int TotalCount { get; } = totalCount;
    public int PageIndex { get; } = pageIndex;
    public int PageSize { get; } = pageSize;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageIndex > PaginationConstants.DefaultPageIndex;
    public bool HasNextPage => PageIndex + 1 < TotalPages;
}
