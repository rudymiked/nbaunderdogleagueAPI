using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface IDraftRepository
    {
        Dictionary<User, List<string>> DraftTeam(User user);
        List<DraftEntity> SetupDraft(string id);
    }

    public class DraftRepository : IDraftRepository
    {
        private readonly IDraftDataAccess _draftDataAccess;
        public DraftRepository(IDraftDataAccess draftDataAccess)
        {
            _draftDataAccess = draftDataAccess;
        }
        public Dictionary<User, List<string>> DraftTeam(User user)
        {
            return _draftDataAccess.DraftTeam(user);
        }
        public List<DraftEntity> SetupDraft(string id)
        {
            return _draftDataAccess.SetupDraft(id);
        }
    }
}
