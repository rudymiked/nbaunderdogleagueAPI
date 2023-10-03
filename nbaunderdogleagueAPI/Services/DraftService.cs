using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface IDraftService
    {
        string DraftTeam(User user);
        List<DraftEntity> SetupDraft(SetupDraftRequest setupDraftRequest);
        List<UserEntity> DraftedTeams(string groupId);
        List<DraftEntity> GetDraft(string groupId);
        List<TeamEntity> GetAvailableTeamsToDraft(string groupId);
        List<DraftResults> GetDraftResults(string groupId);
        string DraftLate(DraftLateRequest draftLateRequest);
    }
    public class DraftService : IDraftService
    {
        private readonly IDraftRepository _draftRepository;
        public DraftService(IDraftRepository draftRepository)
        {
            _draftRepository = draftRepository;
        }
        public string DraftTeam(User user)
        {
            return _draftRepository.DraftTeam(user);
        }
        public List<DraftEntity> SetupDraft(SetupDraftRequest setupDraftRequest)
        {
            return _draftRepository.SetupDraft(setupDraftRequest);
        }
        public List<UserEntity> DraftedTeams(string groupId)
        {
            return _draftRepository.DraftedTeams(groupId);
        }
        public List<DraftEntity> GetDraft(string groupId)
        {
            return _draftRepository.GetDraft(groupId);
        }
        public List<TeamEntity> GetAvailableTeamsToDraft(string groupId)
        {
            return _draftRepository.GetAvailableTeamsToDraft(groupId);
        }
        public List<DraftResults> GetDraftResults(string groupId)
        {
            return _draftRepository.GetDraftResults(groupId);
        }        
        public string DraftLate(DraftLateRequest draftLateRequest)
        {
            return _draftRepository.DraftLate(draftLateRequest);
        }
    }
}
