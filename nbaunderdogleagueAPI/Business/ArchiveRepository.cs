using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.Business
{
    public interface IArchiveRepository
    {
        List<SeasonArchiveEntity> ArchiveCurrentSeason(string groupId);
        List<SeasonArchiveEntity> GetSeasonArchive(string groupId);
        SeasonArchiveEntity ArchiveUser(SeasonArchiveEntity userArchive);
        List<ArchiveSummary> GetArchiveSummary(string email);
        List<SeasonArchiveEntity> UpdateArchives();
    }
    public class ArchiveRepository : IArchiveRepository
    {
        private readonly IArchiveDataAccess _archiveDataAccess;
        public ArchiveRepository(IArchiveDataAccess archiveDataAccess)
        {
            _archiveDataAccess = archiveDataAccess;
        }
        public List<SeasonArchiveEntity> ArchiveCurrentSeason(string groupId)
        {
            return _archiveDataAccess.ArchiveCurrentSeason(groupId);
        }
        public List<SeasonArchiveEntity> GetSeasonArchive(string groupId)
        {
            return _archiveDataAccess.GetSeasonArchive(groupId);
        }
        public SeasonArchiveEntity ArchiveUser(SeasonArchiveEntity userArchive)
        {
            return _archiveDataAccess.ArchiveUser(userArchive);
        }
        public List<ArchiveSummary> GetArchiveSummary(string email)
        {
            return _archiveDataAccess.GetArchiveSummary(email);
        }
        public List<SeasonArchiveEntity> UpdateArchives()
        {
            return _archiveDataAccess.UpdateArchives();
        }
    }
}
