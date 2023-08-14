using nbaunderdogleagueAPI.Models.PlayerStatistics;

namespace nbaunderdogleagueAPI.DataAccess.Helpers
{
    public static class PlayerUtils
    {
        public static double CalculateCompiledPlayerScore(PlayerStatistics playerStatistics)
        {
            return 1;
        }

        public static double Average(int value, double? average, int n)
        {
            if (!average.HasValue) return value;
            return Math.Round(((double)average * (n - 1) + value) / n, 2);
        }

        public static double Average(double value, double? average, int n)
        {
            if (!average.HasValue) return value;
            return Math.Round(((double)average * (n - 1) + value) / n, 2);
        }

        public static int Max(int value, int? max)
        {
            if (!max.HasValue) return value;
            return Math.Max((int)max, value);
        }        
        
        public static double Max(double value, double? max)
        {
            if (!max.HasValue) return value;
            return Math.Round(Math.Max((double)max, value), 2);
        }

        public static int Min(int value, int? min)
        {
            if (!min.HasValue) return value;
            return Math.Min((int)min, value);
        }       
        
        public static double Min(double value, double? min)
        {
            if (!min.HasValue) return value;
            return Math.Round(Math.Min((double)min, value), 2);
        }
    }
}
