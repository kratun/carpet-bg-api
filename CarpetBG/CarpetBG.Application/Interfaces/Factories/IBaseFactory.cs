using CarpetBG.Shared;

namespace CarpetBG.Application.Interfaces.Factories;

public interface IBaseFactory
{
    PaginatedResult<T> CreatePaginatedResult<T>(
        IReadOnlyList<T> items,
        int totalCount,
        int targetPageIndex,
        int targetPageSize);
}
