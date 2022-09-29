using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface IDraftService
    {
        Dictionary<User, List<string>> DraftTeam(User user);
        List<DraftEntity> SetupDraft(string id);
    }
    public class DraftService : IDraftService
    {
        private readonly IDraftRepository _draftRepository;
        public DraftService(IDraftRepository draftRepository)
        {
            _draftRepository = draftRepository;
        }
        public Dictionary<User, List<string>> DraftTeam(User user)
        {
            return _draftRepository.DraftTeam(user);
        }
        public List<DraftEntity> SetupDraft(string id)
        {
            return _draftRepository.SetupDraft(id);
        }
    }
}
