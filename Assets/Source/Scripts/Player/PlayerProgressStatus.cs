using Assets.Source.Scripts.Enums;

namespace Assets.Source.Scripts.Player
{
    public class PlayerProgressStatus
    {
        public GameMode CurrentMode { get; }
        public DifficultyLevel CurrentDifficulty { get; }

        public int RoundNumber { get; }
        public bool IsEasyCompleted { get; }
        public bool IsMediumCompleted { get; }
        public bool IsHardCompleted { get; }

        public bool IsFreeModeAvailable =>
            IsEasyCompleted && IsMediumCompleted && IsHardCompleted;

        public PlayerProgressStatus(
            int roundNumber,
            GameMode currentMode,
            DifficultyLevel currentDifficulty,
            bool isEasyCompleted,
            bool isMediumCompleted,
            bool isHardCompleted)
        {
            RoundNumber = roundNumber;
            CurrentMode = currentMode;
            CurrentDifficulty = currentDifficulty;
            IsEasyCompleted = isEasyCompleted;
            IsMediumCompleted = isMediumCompleted;
            IsHardCompleted = isHardCompleted;
        }
    }
}