namespace CarpetBG.Application.Interfaces.Common;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateTime ToUtc(DateTime localDate);
    DateTime FromUtc(DateTime utcDate);
}
