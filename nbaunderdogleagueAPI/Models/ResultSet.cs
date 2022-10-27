namespace nbaunderdogleagueAPI.Models
{
    public class ResultSet
    {
        public string name { get; set; }
        public List<string> headers { get; set; }
        public List<List<object>> rowSet { get; set; }
    }
}