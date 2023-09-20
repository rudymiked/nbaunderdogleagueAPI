// Links:
// https://hackastat.eu/en/learn-a-stat-box-plus-minus-and-vorp/

namespace nbaunderdogleagueAPI.Models
{
    public class AdvancedPlayerStats
    {
        private readonly PlayerStatisticsEntity _player;

        public AdvancedPlayerStats(PlayerStatisticsEntity playerStatisticsEntity)
        {
            _player = playerStatisticsEntity;
        }
        public PlayerStatisticsEntity PlayerStatisticsEntity { get { return _player; } }
        public double EFF { get { return CalculateEffeciency(); } }
        public double VORP { get { return CalculateVORP(); } }
        public double ReplacementLevel { get { return CalculateReplacementLevel(); } }
        public double BPM { get { return CalculateBPM(); } } // BoxPlusMinus
        private double CalculateEffeciency()
        {
            //(Points + Rebounds + Assists + Steals + Blocks) - ((Field Goals Attempted - Field Goals Made) + (Free Throws Attempted - Free Throws Made) + Turnovers)
            return (_player.AveragePoints + _player.AverageRebounds + _player.AverageSteals + _player.AverageBlocks) 
                    - ((_player.AverageFieldGoalAttempts - _player.AverageFieldGoalsMade)
                    - (_player.AverageFreeThrowsAttempted - _player.AverageFreeThrowsMade)
                    + _player.AverageTurnovers);
        }
        private double CalculateVORP()
        {
            // (eff - replacement_level) * minutes
            return (EFF - ReplacementLevel) * _player.AverageMinutes;
        }
        private double CalculateReplacementLevel()
        {
            // plusmins - 2.0
            return _player.AveragePlusMinus - 2.0;
        }
        private double CalculateBPM() // BoxPlusMinus
        {
            double gBPM1 = (_player.TotalMinutes / (_player.GamesPlayed + 2)); // * corrective factor?
            double gBPM2 = 0;

            return 0;
        }
    }
}
