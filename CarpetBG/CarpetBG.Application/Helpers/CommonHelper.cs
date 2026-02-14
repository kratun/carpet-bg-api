using System.Globalization;

using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.Helpers;

public static class CommonHelper
{
    public static string ConvertToMeasurment(decimal? measurment)
    {
        return measurment?.ToString("F3", CultureInfo.InvariantCulture) ?? string.Empty;
    }

    public static string ConvertToMoney(decimal? money)
    {
        return money?.ToString("F2", CultureInfo.InvariantCulture) ?? string.Empty;
    }

    public static string ConvertToString(OrderItemStatuses status)
    {
        return status.ToString();
    }

    public static string ConvertToString(OrderStatuses status)
    {
        return status.ToString();
    }

    public static string ConvertToDate(DateTime? date)
    {
        return date?.ToString("dd.MM.yyyy") ?? string.Empty;
    }

    public static string ConvertToDateWithTime(DateTime? date)
    {
        return date?.ToString("dd.MM.yyyy HH:mm") ?? string.Empty;
    }

    public static string ConvertToString(long orderNumber)
    {
        return orderNumber.ToString("D6");
    }
}
