﻿using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Services
{
    public interface IArchiveService
    {
        List<SeasonArchiveEntity> ArchiveCurrentSeason(string groupId);
        List<SeasonArchiveEntity> GetSeasonArchive(string groupId);
        SeasonArchiveEntity ArchiveUser(SeasonArchiveEntity userArchive);
    }
    public class ArchiveService : IArchiveService
    {
        private readonly IArchiveRepository _archiveRepository;
        public ArchiveService(IArchiveRepository archiveRepository)
        {
            _archiveRepository = archiveRepository;
        }
        public List<SeasonArchiveEntity> ArchiveCurrentSeason(string groupId)
        {
            return _archiveRepository.ArchiveCurrentSeason(groupId);
        }
        public List<SeasonArchiveEntity> GetSeasonArchive(string groupId)
        {
            return _archiveRepository.GetSeasonArchive(groupId);
        }
        public SeasonArchiveEntity ArchiveUser(SeasonArchiveEntity userArchive)
        {
            return _archiveRepository.ArchiveUser(userArchive);
        }
    }
}
