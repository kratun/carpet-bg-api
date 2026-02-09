using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.Interfaces.Common;

public interface IValidator<T>
{
    /// <summary>
    /// Returns null or empty string if validation succeeds;
    /// otherwise returns an error message describing the failure.
    /// </summary>
    string? Validate(T instance, OrderStatuses? orderStatus = null);
}
