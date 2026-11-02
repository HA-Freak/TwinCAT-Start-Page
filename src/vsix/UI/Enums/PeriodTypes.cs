namespace TwinCAT.XAE.StartPage.UI.Enums
{
    using System;

    using Luminous.Code.Extensions.Enums;

    using Luminous.Code.Extensions.Dates;

    using static Dates.DateMethods;

    internal static class PeriodTypes
    {
        public static PeriodType CalculatePeriodType(bool pinned, DateTime currentDate, DateTime comparisonDate)
        {
            if (currentDate < comparisonDate)
            {
                throw new ArgumentOutOfRangeException("PeriodType: comparison date can't be in the future");
            }

            if (pinned)
            {
                return PeriodType.Pinned;
            }

            var daysAgo = DaysAgo(currentDate, comparisonDate);

            if (daysAgo == 0)
                return PeriodType.Today;

            if (daysAgo == 1)
                return PeriodType.Yesterday;

            if (comparisonDate >= currentDate.FirstDayOfWeek())
                return PeriodType.ThisWeek;

            if (comparisonDate >= currentDate.FirstDayOfPreviousWeek())
                return PeriodType.LastWeek;

            if (comparisonDate >= currentDate.FirstDayOfMonth())
                return PeriodType.ThisMonth;

            if (comparisonDate >= currentDate.FirstDayOfPreviousMonth())
                return PeriodType.LastMonth;

            return PeriodType.Older;
        }

        public static string ToName(this PeriodType period)
            => period.GetName();
    }
}
