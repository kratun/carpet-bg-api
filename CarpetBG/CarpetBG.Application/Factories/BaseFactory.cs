using CarpetBG.Application.Helpers;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Shared;

namespace CarpetBG.Application.Factories;

public abstract class BaseFactory : IBaseFactory
{
    public PaginatedResult<T> CreatePaginatedResult<T>(
        IReadOnlyList<T> items,
        int totalCount,
        int targetPageIndex,
        int targetPageSize)
    {
        var normalizedPageIndex = PaginationHelper.NormalizePageIndex(
            targetPageIndex,
            targetPageSize,
            totalCount);

        return new PaginatedResult<T>(
            items,
            totalCount,
            normalizedPageIndex,
            targetPageSize);
    }
}
