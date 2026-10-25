using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.YG;
using System;
using YG;

namespace Assets.Source.Scripts.Player
{
    public class PlayerProgress
    {
        private const int EasyEndNumberLevel = 5;
        private const int MediumEndNumberLevel = 11;
        private const int HardEndNumberLevel = 17;

        private int _currentRoundNumber;
        private DifficultyLevel _currentDifficultyLevel;

        public bool IsInitialized { get; private set; }
        public PlayerProgressStatus Current { get; private set; }
        public GameMode CurrentMode { get; private set; }
        public DifficultyLevel CurrentDifficultyLevel => _currentDifficultyLevel;
        public int CurrentRoundNumber => _currentRoundNumber;
        public bool IsEasyCompleted { get; private set; }
        public bool IsMediumCompleted { get; private set; }
        public bool IsHardCompleted { get; private set; }

        public bool IsFreeModeAvailable =>
            IsEasyCompleted && IsMediumCompleted && IsHardCompleted;

        public void Initialize()
        {
            _currentRoundNumber = YG2.saves.GetRoundNumber();

            Evaluate();
            LoadOrDetermineDifficulty();

            Current = new PlayerProgressStatus(CurrentMode, _currentRoundNumber);
            IsInitialized = true;
        }

        private void Evaluate()
        {
            IsEasyCompleted = _currentRoundNumber > EasyEndNumberLevel;
            IsMediumCompleted = _currentRoundNumber > MediumEndNumberLevel;
            IsHardCompleted = _currentRoundNumber > HardEndNumberLevel;
            CurrentMode = DetermineCurrentMode(_currentRoundNumber);
        }

        private void LoadOrDetermineDifficulty()
        {
            GameModeSection saved = YG2.saves.Mode;

            if (saved.HasSavedMode && saved.GetGameMode() == CurrentMode)
            {
                _currentDifficultyLevel = saved.GetDifficultyLevel();

                return;
            }

            DetermineCurrentDifficultyLevel();
            //YG2.saves.SaveMode(CurrentMode, _currentDifficultyLevel);
        }

        private void DetermineCurrentDifficultyLevel()
        {
            switch (CurrentMode)
            {
                case GameMode.Easy:
                    _currentDifficultyLevel = DifficultyLevel.Easy;
                    break;

                case GameMode.Medium:
                    _currentDifficultyLevel = DifficultyLevel.Medium;
                    break;

                case GameMode.Hard:
                    _currentDifficultyLevel = DifficultyLevel.MediumHard;
                    break;

                case GameMode.FreeMode:
                    var levels = (DifficultyLevel[])Enum.GetValues(typeof(DifficultyLevel));
                    _currentDifficultyLevel = levels[UnityEngine.Random.Range(0, levels.Length)];
                    break;

                default:
                    _currentDifficultyLevel = DifficultyLevel.Easy;
                    break;
            }
        }

        private GameMode DetermineCurrentMode(int roundNumber)
        {
            if (roundNumber <= EasyEndNumberLevel)
                return GameMode.Easy;

            if (roundNumber <= MediumEndNumberLevel)
                return GameMode.Medium;

            if (roundNumber <= HardEndNumberLevel)
                return GameMode.Hard;

            return GameMode.FreeMode;
        }

        public bool IsModeAvailable(GameMode mode) => mode switch
        {
            GameMode.Tutorial => true,
            GameMode.Easy => true,
            GameMode.Medium => IsEasyCompleted,
            GameMode.Hard => IsMediumCompleted,
            GameMode.FreeMode => IsFreeModeAvailable,
            _ => false
        };

        public bool IsModeCompleted(GameMode mode) => mode switch
        {
            GameMode.Easy => IsEasyCompleted,
            GameMode.Medium => IsMediumCompleted,
            GameMode.Hard => IsHardCompleted,
            _ => false
        };
    }
}