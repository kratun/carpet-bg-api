namespace CarpetBG.Application.Helpers;

public static class PaginationHelper
{
    public static int NormalizePageIndex(
        int targetPageIndex,
        int pageSize,
        int totalCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        if (totalCount <= 0)
        {
            return 0;
        }

        var lastPageIndex = (int)Math.Floor((totalCount - 1) / (double)pageSize);

        if (targetPageIndex < 0)
        {
            return 0;
        }

        if (targetPageIndex > lastPageIndex)
        {
            return lastPageIndex;
        }

        return targetPageIndex;
    }
}
