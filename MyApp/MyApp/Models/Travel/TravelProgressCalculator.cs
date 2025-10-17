using System;

namespace MyApp.Models.Travel
{
    public static class TravelProgressCalculator
    {
        public static int CalculateProgressPercentage(Travel travel, DateOnly referenceDate)
        {
            if (travel.DurationDays <= 0)
            {
                return 0;
            }

            if (referenceDate <= travel.StartDate)
            {
                return 0;
            }

            if (referenceDate >= travel.EndDate)
            {
                return 100;
            }

            int elapsedDays = referenceDate.DayNumber - travel.StartDate.DayNumber + 1;

            double ratio = (double)elapsedDays / travel.DurationDays;
            double percentage = Math.Clamp(ratio * 100d, 0d, 100d);

            int progress = (int)Math.Round(percentage, MidpointRounding.AwayFromZero);

            return progress;
        }
    }
}
