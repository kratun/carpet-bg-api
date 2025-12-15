using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.Helpers;

public static class OrderItemHelper
{
    public static decimal CalculateAmount(decimal price, decimal? width, decimal? height, decimal? diagonal, IEnumerable<IAddition> additions)
    {
        var currentAmount = decimal.Zero;
        if (width.HasValue && height.HasValue)
        {
            currentAmount = width.Value * height.Value * price;
        }

        if (diagonal.HasValue)
        {
            currentAmount = diagonal.Value * price;
        }

        currentAmount = ApplyAdditions(currentAmount, additions);

        return currentAmount;
    }

    private static decimal ApplyAdditions(decimal price, IEnumerable<IAddition> additions)
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
}
