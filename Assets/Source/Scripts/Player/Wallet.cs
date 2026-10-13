using System;
using YG;

namespace Assets.Source.Scripts.Player
{
    public class Wallet
    {
        private readonly string leaderboardName = "GameLB";

        private int _currentScore;
        private int _totalScore;

        private Wallet()
        {
            _totalScore = YG2.saves.Points;
            _currentScore = 0;

            TotalScoreChanged?.Invoke(_totalScore);
        }

        public event Action<int> TotalScoreChanged;
        public event Action<int> LastRoundScoreChanged;

        public int TotalScore => _totalScore;
        public int LastRoundScore { get; private set; }
        public int DisplayScore => _totalScore + _currentScore;

        public void AddPoints(int value)
        {
            if (value < 0)
            {
                throw new ArgumentException(
                    "The value cannot be equal to or less than zero.");
            }

            _currentScore += value;

            TotalScoreChanged?.Invoke(DisplayScore);
        }

        public void ConfirmPoints()
        {
            LastRoundScore = _currentScore;
            _totalScore += _currentScore;

            YG2.saves.SaveScore(_totalScore);
            LastRoundScoreChanged?.Invoke(LastRoundScore);
            YG2.SetLeaderboard(leaderboardName, YG2.saves.Points);
        }

        public void SpendPoints(int price)
        {
            if (price < 0)
                throw new ArgumentException("The price cannot be less than zero.");

            if (price == 0)
                return;

            if (CanAfford(price) == false)
                return;

            _totalScore -= price;

            YG2.saves.DecreaseScore(_totalScore);
            TotalScoreChanged?.Invoke(_totalScore);
        }

        public void Reset()
        {
            _currentScore = 0;
        }

        public bool CanAfford(int value)
        {
            if (value < 0)
                return false;

            return _totalScore >= value;
        }
    }
}