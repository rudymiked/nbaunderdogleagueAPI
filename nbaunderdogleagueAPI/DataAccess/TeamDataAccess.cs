using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface ITeamDataAccess
    {
        List<Team> GetTeamData();
    }
    public class TeamDataAccess : ITeamDataAccess
    {
        public TeamDataAccess() { }
        public List<Team> GetTeamData()
        {
            List<Team> teamData = new()
            {
                new Team()
                {
                    TeamId = 001,
                    TeamName = "Lakers",
                    TeamCity = "Los Angeles",
                    ProjectedWin = 41,
                    ProjectedLoss = 41,
                    Win = 20,
                    Loss = 10,
                    Playoffs = "C"
                    
                },
                new Team()
                {
                    TeamId = 002,
                    TeamName = "Suns",
                    TeamCity = "Pheonix",
                    ProjectedWin = 25,
                    ProjectedLoss = 30,
                    Win = 15,
                    Loss = 15,
                    Playoffs = ""
                }
            };

            return teamData;
        }
    }
}
