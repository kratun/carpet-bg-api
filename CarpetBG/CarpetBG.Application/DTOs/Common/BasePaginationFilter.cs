using CarpetBG.Shared.Constants;

namespace CarpetBG.Application.DTOs.Common;

public abstract class BasePaginationFilter
{
    public int PageIndex { get; set; } = PaginationConstants.DefaultPageIndex;
    public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
}
