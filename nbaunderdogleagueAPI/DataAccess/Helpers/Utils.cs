namespace nbaunderdogleagueAPI.DataAccess.Helpers
{
    public class Utils
    {
        public static double CalculateScore(int ProjectedWin, int ProjectedLoss, int Wins, int Losses, int PlayoffWins = 0)
        {
            double projectedDiff = (double)(ProjectedWin / (double)(ProjectedWin + ProjectedLoss));
            double actualDiff = (double)(Wins / (double)(Wins + Losses));
            double score = (double)(actualDiff / (double)projectedDiff);

            return double.IsNaN(Math.Round(score, 2)) ? 0.0 : Math.Round(score, 2);
        }
    }
}
