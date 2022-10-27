namespace nbaunderdogleagueAPI.Models
{
    public class TeamStats
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public string TeamCity { get; set; }
        public string Conference { get; set; }
        public int Standing { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public double Ratio { get; set; }
        public int? Streak { get; set; }
        public int? ClinchedPlayoffBirth { get; set; }

        public static IEnumerable<TeamStats> CorrectStanding(IEnumerable<TeamStats> team)
        {
            /** LeagueStanding API in older seasons provides no information for teams standing.
            This sets their standing based on the teams win/loss ratio in comparison to other teams **/

            if (team.All(t => t.Standing == 0)) {
                int index = 0;

                return team.OrderByDescending(t => t.Ratio).Select((t, standing) => { t.Standing = ++index; return t; });
            }
            return team;
        }
        public override string ToString()
        {
            string output = String.Format("TeamId: {0} \n" +
                "TeamName: {1} \n" +
                "TeamName: {2} \n" +
                "Conference: {3} \n" +
                "Standing: {4} \n" +
                "Wins: {5} \n" +
                "Losses: {6} \n" +
                "Ratio: {7} \n" +
                "Streak: {8} \n",
                "ClinchedPlayoffBirth: {9} \n",
                TeamID, TeamName, TeamCity, Conference, Standing,
                Wins, Losses, Ratio, Streak, ClinchedPlayoffBirth);

            return output;
        }
    }
}