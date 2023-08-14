using Azure.Data.Tables;

namespace nbaunderdogleagueAPI.Models.RapidAPI_NBA
{
    public class RapidAPI_NBA
    {
        public class TimeoutEntity : ITableEntity
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public DateTimeOffset? NextTimeAvailableDateTime { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public Azure.ETag ETag { get; set; }
        }
        public class GameResponse
        {
            public List<Game.Response> Games { get; set; }
            public int RequestsRemaining { get; set; }
        }
        public class TeamStatsResponse
        {
            public List<TeamStats> TeamStats { get; set; }
            public int RequestsRemaining { get; set; }
        }
        public class RapidAPIContent
        {
            public string Content { get; set; }
            public int RequestsRemaining { get; set; }
        }

        public class Game
        {
            public class Arena
            {
                public object name { get; set; }
                public object city { get; set; }
                public object state { get; set; }
                public object country { get; set; }
            }

            public class Date
            {
                public DateTime start { get; set; }
                public object end { get; set; }
                public object duration { get; set; }
            }

            public class Home
            {
                public int id { get; set; }
                public string name { get; set; }
                public string nickname { get; set; }
                public string code { get; set; }
                public string logo { get; set; }
                public int win { get; set; }
                public int loss { get; set; }
                public Series series { get; set; }
                public List<string> linescore { get; set; }
                public int? points { get; set; }
            }

            public class Parameters
            {
                public string date { get; set; }
            }

            public class Periods
            {
                public int current { get; set; }
                public int total { get; set; }
                public bool endOfPeriod { get; set; }
            }

            public class Response
            {
                public int id { get; set; }
                public string league { get; set; }
                public int season { get; set; }
                public Date date { get; set; }
                public int stage { get; set; }
                public Status status { get; set; }
                public Periods periods { get; set; }
                public Arena arena { get; set; }
                public Teams teams { get; set; }
                public Scores scores { get; set; }
                public List<object> officials { get; set; }
                public object timesTied { get; set; }
                public object leadChanges { get; set; }
                public object nugget { get; set; }
            }

            public class Root
            {
                public string get { get; set; }
                public Parameters parameters { get; set; }
                public List<object> errors { get; set; }
                public int results { get; set; }
                public List<Response> response { get; set; }
            }

            public class Scores
            {
                public Visitors visitors { get; set; }
                public Home home { get; set; }
            }

            public class Series
            {
                public int win { get; set; }
                public int loss { get; set; }
            }

            public class Status
            {
                public object clock { get; set; }
                public bool halftime { get; set; }
                public int @short { get; set; }
                public string @long { get; set; }
            }

            public class Teams
            {
                public Visitors visitors { get; set; }
                public Home home { get; set; }
            }

            public class Visitors
            {
                public int id { get; set; }
                public string name { get; set; }
                public string nickname { get; set; }
                public string code { get; set; }
                public string logo { get; set; }
                public int win { get; set; }
                public int loss { get; set; }
                public Series series { get; set; }
                public List<string> linescore { get; set; }
                public int? points { get; set; }
            }
        }

        public class Standings
        {
            public class Conference
            {
                public string name { get; set; }
                public int rank { get; set; }
                public int win { get; set; }
                public int loss { get; set; }
            }

            public class Division
            {
                public string name { get; set; }
                public int rank { get; set; }
                public int win { get; set; }
                public int loss { get; set; }
                public string gamesBehind { get; set; }
            }

            public class Loss
            {
                public int home { get; set; }
                public int away { get; set; }
                public int total { get; set; }
                public string percentage { get; set; }
                public int lastTen { get; set; }
            }

            public class Parameters
            {
                public string league { get; set; }
                public string season { get; set; }
            }

            public class Response
            {
                public string league { get; set; }
                public int season { get; set; }
                public Team team { get; set; }
                public Conference conference { get; set; }
                public Division division { get; set; }
                public Win win { get; set; }
                public Loss loss { get; set; }
                public string gamesBehind { get; set; }
                public int streak { get; set; }
                public bool winStreak { get; set; }
                public object tieBreakerPoints { get; set; }
            }

            public class Root
            {
                public string get { get; set; }
                public Parameters parameters { get; set; }
                public List<object> errors { get; set; }
                public int results { get; set; }
                public List<Response> response { get; set; }
                public List<TeamStats> ExtractTeamStats(ILogger logger)
                {
                    IEnumerable<TeamStats> output = new List<TeamStats>();

                    try {
                        // order by wins, to calculate standing
                        List<Response> orderedResponse = response.OrderByDescending(r => r.win.total).ToList();

                        int i = 1;
                        output = orderedResponse.Select(r => new TeamStats() {
                            TeamID = r.team.id,
                            TeamName = r.team.nickname,
                            TeamCity = r.team.name.Replace(r.team.nickname, "").Trim(),
                            Conference = r.conference.name,
                            Standing = i++,
                            Wins = r.win.total,
                            Losses = r.loss.total,
                            Ratio = double.Parse(r.win.percentage),
                            Streak = r.streak,
                            //ClinchedPlayoffBirth = 0, // not in Rapid API endpoint
                            Logo = r.team.logo,
                            //PlayoffWins = 0 // not in Rapid API endpoint
                        });

                        return output.ToList();
                    } catch (Exception ex) {
                        logger.LogError(ex, ex.Message);
                    }

                    return new List<TeamStats>();
                }
            }

            public class Team
            {
                public int id { get; set; }
                public string name { get; set; }
                public string nickname { get; set; }
                public string code { get; set; }
                public string logo { get; set; }
            }

            public class Win
            {
                public int home { get; set; }
                public int away { get; set; }
                public int total { get; set; }
                public string percentage { get; set; }
                public int lastTen { get; set; }
            }
        }
    }
}
