﻿using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface ILeagueService
    {
        List<LeagueStandings> GetLeagueStandings();
        LeagueInfo CreateLeague(string name, string ownerEmail);
        string JoinLeague(string id, string email);
        LeagueEntity GetLeague(string leagueId);
    }
    public class LeagueService : ILeagueService
    {
        private readonly ILeagueRepository _leagueRepository;
        public LeagueService(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }
        public List<LeagueStandings> GetLeagueStandings()
        {
            return _leagueRepository.GetLeagueStandings();
        }
        public LeagueInfo CreateLeague(string name, string ownerEmail)
        {
            return _leagueRepository.CreateLeague(name, ownerEmail);
        }
        public string JoinLeague(string id, string email)
        {
            return _leagueRepository.JoinLeague(id, email);
        }
        public LeagueEntity GetLeague(string leagueId)
        {
            return _leagueRepository.GetLeague(leagueId);
        }
    }
}
