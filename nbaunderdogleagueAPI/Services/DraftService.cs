﻿using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface IDraftService
    {
        Dictionary<User, string> DraftTeam(User user);
        List<DraftEntity> SetupDraft(string groupId);
        List<User> DraftedTeams(string groupId);
    }
    public class DraftService : IDraftService
    {
        private readonly IDraftRepository _draftRepository;
        public DraftService(IDraftRepository draftRepository)
        {
            _draftRepository = draftRepository;
        }
        public Dictionary<User, string> DraftTeam(User user)
        {
            return _draftRepository.DraftTeam(user);
        }
        public List<DraftEntity> SetupDraft(string groupId)
        {
            return _draftRepository.SetupDraft(groupId);
        }
        public List<User> DraftedTeams(string groupId)
        {
            return _draftRepository.DraftedTeams(groupId);
        }
    }
}
