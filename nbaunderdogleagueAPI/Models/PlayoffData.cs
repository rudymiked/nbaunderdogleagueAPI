namespace nbaunderdogleagueAPI.Models
{
    public class PlayoffData
    {
        public string TeamName { get; set; }
        public int PlayoffWins { get; set; }
        public int ClinchedPlayoffBirth { get; set; }

        public static Dictionary<string, (int PlayoffWins, int ClinchedPlayoffBirth)> PlayoffDataDict = new Dictionary<string, (int PlayoffWins, int ClinchedPlayoffBirth)>();

        public static void AddToDictionary(PlayoffData data)
        {
            PlayoffDataDict[data.TeamName] = (data.PlayoffWins, data.ClinchedPlayoffBirth);
        }
    }
}
