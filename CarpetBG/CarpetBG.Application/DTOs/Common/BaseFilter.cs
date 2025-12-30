namespace CarpetBG.Application.DTOs.Common;

public abstract class BaseFilter<T> : BasePaginationFilter
{
    public string? SearchTerm { get; set; }
    public T? Filter { get; set; }
}
