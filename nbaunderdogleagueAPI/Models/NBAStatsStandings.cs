namespace nbaunderdogleagueAPI.Models
{
    public class NBAStatsStandings
    {
        public class Parameters
        {
            public string LeagueID { get; set; }

            public string SeasonYear { get; set; }

            public string SeasonType { get; set; }
        }

        public class ResultSets
        {
            public string name { get; set; }

            public List<object> headers { get; set; }

            public List<List<object>> rowSet { get; set; }
        }

        public class RootObject
        {
            public string resource { get; set; }

            public Parameters parameters { get; set; }

            public ResultSets resultSets { get; set; }
        }
    }
}
