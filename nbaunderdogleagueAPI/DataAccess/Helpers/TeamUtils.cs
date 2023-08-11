namespace nbaunderdogleagueAPI.DataAccess.Helpers
{
    public class TeamUtils
    {
        public static double CalculateTeamScore(int projectedWin, int projectedLoss, int wins, int losses, int? playoffWins = 0)
        {
            double projectedDiff = (double)projectedWin / (projectedWin + projectedLoss);
            double actualDiff = (double)(wins + playoffWins) / (wins + losses); // bonus for playoff wins
            double score = actualDiff / projectedDiff;

            double roundedScore = Math.Round(score, 2);

            return double.IsNaN(roundedScore) ? 0.0 : roundedScore;
        }
    }
}
