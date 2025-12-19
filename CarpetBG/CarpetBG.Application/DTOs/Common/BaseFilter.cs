namespace CarpetBG.Application.DTOs.Common;

public abstract class BaseFilter : BasePaginationFilter
{
    public string? SearchTerm { get; set; }
}
