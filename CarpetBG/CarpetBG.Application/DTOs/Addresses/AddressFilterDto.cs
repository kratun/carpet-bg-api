using CarpetBG.Domain.Constants;

namespace CarpetBG.Application.DTOs.Addresses;

public class AddressFilterDto
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = PaginationConstants.DefaultPageNumber;
    public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
}
