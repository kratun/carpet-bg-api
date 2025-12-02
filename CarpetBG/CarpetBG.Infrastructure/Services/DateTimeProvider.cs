using CarpetBG.Application.Interfaces.Common;

namespace CarpetBG.Infrastructure.Services;

public class DateTimeProvider(TimeZoneInfo timeZone) : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime ToUtc(DateTime localTime)
    {
        if (localTime.Kind == DateTimeKind.Utc)
        {
            return localTime;
        }

        if (localTime.Kind != DateTimeKind.Unspecified)
        {
            throw new ArgumentException("Expected DateTimeKind.Unspecified");
        }

        return TimeZoneInfo.ConvertTimeToUtc(localTime, timeZone);
    }

    public DateTime FromUtc(DateTime utcTime)
    {
        if (utcTime.Kind != DateTimeKind.Utc)
        {
            utcTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
        }

        var local = TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone);
        return DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
    }
}
