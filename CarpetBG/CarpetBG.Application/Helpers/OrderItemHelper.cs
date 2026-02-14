using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.Helpers;

public static class OrderItemHelper
{
    public static decimal CalculateAmount(decimal price, decimal? width, decimal? height, IEnumerable<IAddition> additions)
    {
        var currentAmount = decimal.Zero;
        if (width.HasValue && height.HasValue)
        {
            currentAmount = CalculateQuantity(width, height) * price;
        }

        if (width.HasValue && !height.HasValue)
        {
            currentAmount = width.Value * price;
        }

        if (!width.HasValue && height.HasValue)
        {
            currentAmount = height.Value * price;
        }

        currentAmount = ApplyAdditions(currentAmount, additions);

        return currentAmount;
    }

    public static decimal CalculateQuantity(decimal? width, decimal? height)
    {
        var quantity = decimal.Zero;
        if (width.HasValue && height.HasValue)
        {
            quantity = width.Value * height.Value;
        }

        return quantity;
    }

    public static decimal ApplyAdditions(decimal price, IEnumerable<IAddition> additions)
    {
        var currentPercentage = decimal.Zero;

        foreach (var addition in additions)
        {
            if (addition.AdditionType == AdditionTypes.AppliedAsPercentage)
            {
                currentPercentage += addition.Value;
            }

            if (addition.AdditionType == AdditionTypes.AppliedAsAmount)
            {
                var amountAsPercentage = (decimal)(addition.Value * 100) / price;
                currentPercentage += amountAsPercentage;
            }
        }

        var priceWithAdditions = price * (1 + currentPercentage);

        return priceWithAdditions;
    }

    public static string GetMeasurmentData(decimal? width, decimal? height)
    {
        var result = string.Empty;
        if (width.HasValue && height.HasValue)
        {
            result = $"{CommonHelper.ConvertToMeasurment(width)}x{CommonHelper.ConvertToMeasurment(height)}";
        }

        if (width.HasValue && !height.HasValue)
        {
            result = $"{CommonHelper.ConvertToMeasurment(width)}";
        }

        if (!width.HasValue && height.HasValue)
        {
            result = $"{CommonHelper.ConvertToMeasurment(height)}";
        }

        return result;
    }
}
