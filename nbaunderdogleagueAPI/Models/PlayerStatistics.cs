namespace nbaunderdogleagueAPI.Models.PlayerStatistics
{
    public class PlayerStatistics
    {
        public class Root
        {
            public string Get { get; set; }
            public Parameters Parameters { get; set; }
            public List<string> Errors { get; set; }
            public int Results { get; set; }
            public List<Response> Response { get; set; }
        }

        public class PlayerResponse
        {
            public List<Response> Players { get; set; }
            public int RequestsRemaining { get; set; }
        }

        public class Parameters
        {
            public string Team { get; set; }
            public string Season { get; set; }
        }

        public class Response
        {
            public Player Player { get; set; }
            public Team Team { get; set; }
            public Game Game { get; set; }
            public int Points { get; set; }
            public string Pos { get; set; }
            public string Min { get; set; }
            public int Fgm { get; set; }
            public int Fga { get; set; }
            public string Fgp { get; set; }
            public int Ftm { get; set; }
            public int Fta { get; set; }
            public string Ftp { get; set; }
            public int Tpm { get; set; }
            public int Tpa { get; set; }
            public string Tpp { get; set; }
            public int OffReb { get; set; }
            public int DefReb { get; set; }
            public int TotReb { get; set; }
            public int Assists { get; set; }
            public int PFouls { get; set; }
            public int Steals { get; set; }
            public int Turnovers { get; set; }
            public int Blocks { get; set; }
            public string PlusMinus { get; set; }
        }

        public class Player
        {
            public int Id { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
        }

        public class Team
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Nickname { get; set; }
            public string Code { get; set; }
            public string Logo { get; set; }
        }

        public class Game
        {
            public int Id { get; set; }
        }
    }
}
