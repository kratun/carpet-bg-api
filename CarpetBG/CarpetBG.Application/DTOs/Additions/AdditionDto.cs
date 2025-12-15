using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.DTOs.Additions;

public class AdditionDto : IAddition
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public AdditionTypes AdditionType { get; set; }
    public decimal Value { get; set; }
}
