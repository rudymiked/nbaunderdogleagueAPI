using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface IDraftRepository
    {
        string DraftTeam(User user);
        List<DraftEntity> SetupDraft(string groupId);
        List<UserEntity> DraftedTeams(string groupId);
        List<DraftEntity> GetDraft(string groupId);
        List<TeamEntity> GetAvailableTeamsToDraft(string groupId);
    }

    public class DraftRepository : IDraftRepository
    {
        private readonly IDraftDataAccess _draftDataAccess;
        public DraftRepository(IDraftDataAccess draftDataAccess)
        {
            _draftDataAccess = draftDataAccess;
        }
        public string DraftTeam(User user)
        {
            return _draftDataAccess.DraftTeam(user);
        }
        public List<DraftEntity> SetupDraft(string groupId)
        {
            return _draftDataAccess.SetupDraft(groupId);
        }
        public List<UserEntity> DraftedTeams(string groupId)
        {
            return _draftDataAccess.DraftedTeams(groupId);
        }        
        public List<DraftEntity> GetDraft(string groupId)
        {
            return _draftDataAccess.GetDraft(groupId);
        }       
        public List<TeamEntity> GetAvailableTeamsToDraft(string groupId)
        {
            return _draftDataAccess.GetAvailableTeamsToDraft(groupId);
        }
    }
}
