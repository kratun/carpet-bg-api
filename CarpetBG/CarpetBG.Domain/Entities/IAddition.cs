using CarpetBG.Domain.Enums;

namespace CarpetBG.Domain.Entities;

public interface IAddition
{
    string Name { get; set; }
    string NormalizedName { get; set; }
    AdditionTypes AdditionType { get; set; }
    decimal Value { get; set; }
}
