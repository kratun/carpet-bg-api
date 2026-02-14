namespace CarpetBG.Application.Common;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyPagination<T>(
        this IQueryable<T> query,
        int pageIndex,
        int pageSize)
    {
        if (pageIndex < 0 || pageSize <= 0)
        {
            return query;
        }

        return query
            .Skip(pageIndex * pageSize)
            .Take(pageSize);
    }
}
