using Assets.Source.Scripts.Enums;
using System;
using YG;
using Zenject;

namespace Assets.Source.Scripts.Player
{
    public class PlayerProgress : IInitializable
    {
        private const int EasyEndNumberLevel = 5;
        private const int MediumEndNumberLevel = 11;
        private const int HardEndNumberLevel = 17;

        private PlayerProgressStatus _status;
        private bool _isInitialized;

        public PlayerProgressStatus Status => _status;
        public bool IsInitialized => _isInitialized;
        public GameMode CurrentMode => _status?.CurrentMode ?? GameMode.Tutorial;
        public DifficultyLevel CurrentDifficultyLevel => _status?.CurrentDifficulty ?? DifficultyLevel.Easy;
        public int CurrentRoundNumber => _status?.RoundNumber ?? 0;
        public bool IsEasyCompleted => _status?.IsEasyCompleted ?? false;
        public bool IsMediumCompleted => _status?.IsMediumCompleted ?? false;
        public bool IsHardCompleted => _status?.IsHardCompleted ?? false;
        public bool IsFreeModeAvailable => _status?.IsFreeModeAvailable ?? false;

        public void Initialize()
        {
            if (_isInitialized) return;

            int roundNumber = YG2.saves.GetRoundNumber();

            GameMode mode = YG2.saves.HasSavedMode
                ? YG2.saves.LoadMode()
                : DetermineCurrentMode(roundNumber);

            DifficultyLevel difficulty = mode == GameMode.FreeMode
                ? PickRandomDifficulty()
                : GetBaseDifficulty(mode);

            _status = new PlayerProgressStatus(
                roundNumber,
                mode,
                difficulty,
                roundNumber > EasyEndNumberLevel,
                roundNumber > MediumEndNumberLevel,
                roundNumber > HardEndNumberLevel);

            _isInitialized = true;
        }

        public void SaveSelectedMode(GameMode mode)
        {
            if (CurrentMode == mode && YG2.saves.LoadMode() == mode)
                return;

            YG2.saves.SaveMode(mode);

            DifficultyLevel difficulty = mode == GameMode.FreeMode
                ? PickRandomDifficulty()
                : GetBaseDifficulty(mode);

            _status = new PlayerProgressStatus(
                CurrentRoundNumber,
                mode,
                difficulty,
                IsEasyCompleted,
                IsMediumCompleted,
                IsHardCompleted);
        }

        public static DifficultyLevel GetBaseDifficulty(GameMode mode) => mode switch
        {
            GameMode.Easy => DifficultyLevel.Easy,
            GameMode.Medium => DifficultyLevel.Medium,
            GameMode.Hard => DifficultyLevel.MediumHard,
            GameMode.Tutorial => DifficultyLevel.Easy,
            _ => DifficultyLevel.Easy
        };

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

        private static DifficultyLevel PickRandomDifficulty()
        {
            var levels = (DifficultyLevel[])Enum.GetValues(typeof(DifficultyLevel));
            return levels[UnityEngine.Random.Range(0, levels.Length)];
        }

        private static GameMode DetermineCurrentMode(int roundNumber)
        {
            if (roundNumber <= EasyEndNumberLevel) return GameMode.Easy;
            if (roundNumber <= MediumEndNumberLevel) return GameMode.Medium;
            if (roundNumber <= HardEndNumberLevel) return GameMode.Hard;
            return GameMode.FreeMode;
        }
    }
}