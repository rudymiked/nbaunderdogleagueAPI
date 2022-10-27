namespace nbaunderdogleagueAPI.Models
{
    public class LeagueStandingsRootObject
    {
        public List<ResultSet> ResultSets { get; set; }

        public List<TeamStats> ExtractTeamStats(string season, ILogger logger)
        {
            IEnumerable<TeamStats> output = new List<TeamStats>();

            try {
                int year = Convert.ToInt32(season.Substring(0, 4));

                //Conferences where introduced in 1970
                //Before that divisions where used
                if (year > 1969) {
                    output = ResultSets[0].rowSet.Select(s => new TeamStats() {
                        TeamID = Convert.ToInt32(s[2]),
                        TeamName = s[4].ToString(),
                        TeamCity = s[3].ToString(),
                        Conference = s[6].ToString(),
                        Standing = Convert.ToInt32(s[8]),
                        Wins = Convert.ToInt32(s[13]),
                        Losses = Convert.ToInt32(s[14]),
                        Ratio = Convert.ToDouble(s[15]),
                        Streak = (s[34] == null) ? null : (int?)Convert.ToInt32(s[34]),
                        ClinchedPlayoffBirth = Convert.ToInt32(s[40])
                    });
                } else {
                    output = ResultSets[0].rowSet.Select(s => new TeamStats() {
                        TeamID = Convert.ToInt32(s[2]),
                        TeamName = s[4].ToString(),
                        TeamCity = s[3].ToString(),
                        Conference = s[6].ToString(),
                        Standing = Convert.ToInt32(s[8]),
                        Wins = Convert.ToInt32(s[13]),
                        Losses = Convert.ToInt32(s[14]),
                        Ratio = Convert.ToDouble(s[15]),
                        Streak = (s[34] == null) ? null : (int?)Convert.ToInt32(s[34]),
                        ClinchedPlayoffBirth = Convert.ToInt32(s[40])
                    });
                }

            } catch (Exception ex) {
                logger.LogError(ex, ex.Message);
            }

            return output.ToList();
        }

    }
}