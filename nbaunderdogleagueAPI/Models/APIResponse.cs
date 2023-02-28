namespace nbaunderdogleagueAPI.Models
{
    public class APIResponse
    {
        public string Get { get; set; }
        public string[] Parameters { get; set; }
        public string[] Errors { get; set; }
        public int Results { get; set; }
        public List<object> Response { get; set; }
    }
}
