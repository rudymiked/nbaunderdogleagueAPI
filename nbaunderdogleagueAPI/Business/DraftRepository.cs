using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface IDraftRepository
    {
        Dictionary<User, string> DraftTeam(User user);
        List<DraftEntity> SetupDraft(string groupId);
        List<User> DraftedTeams(string groupId);
    }

    public class DraftRepository : IDraftRepository
    {
        private readonly IDraftDataAccess _draftDataAccess;
        public DraftRepository(IDraftDataAccess draftDataAccess)
        {
            _draftDataAccess = draftDataAccess;
        }
        public Dictionary<User, string> DraftTeam(User user)
        {
            return _draftDataAccess.DraftTeam(user);
        }
        public List<DraftEntity> SetupDraft(string groupId)
        {
            return _draftDataAccess.SetupDraft(groupId);
        }
        public List<User> DraftedTeams(string groupId)
        {
            return _draftDataAccess.DraftedTeams(groupId);
        }
    }
}
