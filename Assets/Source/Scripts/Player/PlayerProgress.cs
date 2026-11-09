using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.GameBehaviour;
using System;
using UnityEngine;
using YG;
using Zenject;

namespace Assets.Source.Scripts.Player
{
    public class PlayerProgress : IInitializable
    {
        private RoundSequenceProvider _sequenceProvider;
        private GameModeRuntimeProvider _runtimeProvider;
        private PlayerProgressStatus _status;
        private bool _isInitialized;

        public PlayerProgressStatus Status => _status;
        public bool IsInitialized => _isInitialized;
        public GameMode CurrentMode => _status?.CurrentMode ?? GameMode.Tutorial;
        public DifficultyLevel CurrentDifficultyLevel => _status?.CurrentDifficulty ?? DifficultyLevel.Easy;
        public int CurrentRoundNumber => _status?.RoundNumber ?? 1;
        public bool IsEasyCompleted => _status?.IsEasyCompleted ?? false;
        public bool IsMediumCompleted => _status?.IsMediumCompleted ?? false;
        public bool IsHardCompleted => _status?.IsHardCompleted ?? false;
        public bool IsFreeModeAvailable => _status?.IsFreeModeAvailable ?? false;

        [Inject]
        private void Construct(RoundSequenceProvider sequenceProvider, GameModeRuntimeProvider runtimeProvider)
        {
            Guard.NotNull(sequenceProvider, nameof(sequenceProvider));
            Guard.NotNull(runtimeProvider, nameof(runtimeProvider));

            _sequenceProvider = sequenceProvider;
            _runtimeProvider = runtimeProvider;
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            int roundNumber = YG2.saves.GetRoundNumber();
            if (roundNumber < 1) roundNumber = 1;

            GameMode mode = YG2.saves.HasSavedMode
                ? YG2.saves.LoadMode()
                : GameMode.Tutorial;

            // Восстанавливаем сессионное состояние из сейва (включая данные дебаг-сида)
            RestoreRuntimeFromSaves();

            DifficultyLevel difficulty = mode == GameMode.FreeMode
                ? PickRandomDifficulty()
                : GetBaseDifficulty(mode);

            _status = BuildStatus(roundNumber, mode, difficulty);
            _isInitialized = true;

            Debug.Log($"[PlayerProgress] Init: mode={mode}, round={roundNumber}, " +
                      $"Easy={IsEasyCompleted}, Medium={IsMediumCompleted}, Hard={IsHardCompleted}");
        }

        public void SaveSelectedMode(GameMode mode)
        {
            ValidateMode(mode, nameof(mode));

            if (CurrentMode == mode && YG2.saves.LoadMode() == mode)
                return;

            YG2.saves.SaveMode(mode);

            if (_runtimeProvider.TryGet(mode, out var runtime))
                runtime.ResetAll();

            DifficultyLevel difficulty = mode == GameMode.FreeMode
                ? PickRandomDifficulty()
                : GetBaseDifficulty(mode);

            _status = BuildStatus(1, mode, difficulty);
        }

        public void MarkRoundFinished(GameMode mode, int roundIndex, RoundRating rating, bool keepBest = true)
        {
            ValidateMode(mode, nameof(mode));

            if (_runtimeProvider.TryGet(mode, out var runtime) == false)
                return;

            runtime.MarkRoundFinished(roundIndex, rating, keepBest);
            YG2.saves.SaveRoundRating(mode, roundIndex, rating, keepBest);

            _status = BuildStatus(CurrentRoundNumber, CurrentMode, CurrentDifficultyLevel);
        }

        public int GetCompletedRoundsCount(GameMode mode)
        {
            ValidateMode(mode, nameof(mode));

            if (mode != GameMode.Easy && mode != GameMode.Medium && mode != GameMode.Hard)
                return 0;

            int start = _sequenceProvider.GetStartRoundNumber(mode);
            int end = _sequenceProvider.GetEndRoundNumber(mode);
            int count = 0;

            for (int roundIndex = start; roundIndex <= end; roundIndex++)
            {
                if (YG2.saves.GetRoundRating(mode, roundIndex) != RoundRating.NotCompleted)
                    count++;
            }

            return count;
        }

        public static DifficultyLevel GetBaseDifficulty(GameMode mode)
        {
            ValidateMode(mode, nameof(mode));

            return mode switch
            {
                GameMode.Easy => DifficultyLevel.Easy,
                GameMode.Medium => DifficultyLevel.Medium,
                GameMode.Hard => DifficultyLevel.MediumHard,
                GameMode.Tutorial => DifficultyLevel.Easy,
                _ => DifficultyLevel.Easy
            };
        }

        public bool IsModeAvailable(GameMode mode)
        {
            ValidateMode(mode, nameof(mode));

            return mode switch
            {
                GameMode.Tutorial => true,
                GameMode.Easy => true,
                GameMode.Medium => IsEasyCompleted,
                GameMode.Hard => IsMediumCompleted,
                GameMode.FreeMode => IsFreeModeAvailable,
                _ => false
            };
        }

        private bool IsModeCompleted(GameMode mode)
        {
            if (mode != GameMode.Easy && mode != GameMode.Medium && mode != GameMode.Hard)
                return false;

            int start = _sequenceProvider.GetStartRoundNumber(mode);
            int end = _sequenceProvider.GetEndRoundNumber(mode);

            for (int roundIndex = start; roundIndex <= end; roundIndex++)
            {
                if (YG2.saves.GetRoundRating(mode, roundIndex) == RoundRating.NotCompleted)
                    return false;
            }

            return true;
        }

        private void RestoreRuntimeFromSaves()
        {
            RestoreModeFromSaves(GameMode.Easy);
            RestoreModeFromSaves(GameMode.Medium);
            RestoreModeFromSaves(GameMode.Hard);
        }

        private void RestoreModeFromSaves(GameMode mode)
        {
            if (_runtimeProvider.TryGet(mode, out var runtime) == false)
                return;

            var ratings = YG2.saves.GetRoundRatingsForMode(mode);

            foreach (var pair in ratings)
            {
                if (pair.Value == RoundRating.NotCompleted) continue;

                try
                {
                    runtime.MarkRoundFinished(pair.Key, pair.Value, keepBest: false);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Debug.LogWarning($"[PlayerProgress] Некорректный roundIndex={pair.Key} для {mode}.");
                }
            }
        }

        private PlayerProgressStatus BuildStatus(int roundNumber, GameMode mode, DifficultyLevel difficulty)
        {
            if (roundNumber < 1) roundNumber = 1;

            return new PlayerProgressStatus(
                roundNumber,
                mode,
                difficulty,
                IsModeCompleted(GameMode.Easy),
                IsModeCompleted(GameMode.Medium),
                IsModeCompleted(GameMode.Hard));
        }

        private static DifficultyLevel PickRandomDifficulty()
        {
            var levels = (DifficultyLevel[])Enum.GetValues(typeof(DifficultyLevel));

            Guard.IsTrue(levels.Length > 0, nameof(levels),
                $"В {nameof(DifficultyLevel)} нет ни одного значения.");

            return levels[UnityEngine.Random.Range(0, levels.Length)];
        }

        private static void ValidateMode(GameMode mode, string argumentName)
        {
            Guard.IsTrue(
                Enum.IsDefined(typeof(GameMode), mode),
                argumentName,
                $"Значение {mode} ({(int)mode}) не является допустимым {nameof(GameMode)}.");
        }
    }
}

//namespace Assets.Source.Scripts.Player
//{
//    public class PlayerProgress : IInitializable
//    {
//        private RoundSequenceProvider _sequenceProvider;
//        private PlayerProgressStatus _status;
//        private bool _isInitialized;

//        public PlayerProgressStatus Status => _status;
//        public bool IsInitialized => _isInitialized;
//        public GameMode CurrentMode => _status?.CurrentMode ?? GameMode.Tutorial;
//        public DifficultyLevel CurrentDifficultyLevel => _status?.CurrentDifficulty ?? DifficultyLevel.Easy;
//        public int CurrentRoundNumber => _status?.RoundNumber ?? 0;
//        public bool IsEasyCompleted => _status?.IsEasyCompleted ?? false;
//        public bool IsMediumCompleted => _status?.IsMediumCompleted ?? false;
//        public bool IsHardCompleted => _status?.IsHardCompleted ?? false;
//        public bool IsFreeModeAvailable => _status?.IsFreeModeAvailable ?? false;

//        [Inject]
//        private void Construct(RoundSequenceProvider sequenceProvider)
//        {
//            Guard.NotNull(sequenceProvider, nameof(sequenceProvider));
//            _sequenceProvider = sequenceProvider;
//        }

//        public void Initialize()
//        {
//            if (_isInitialized)
//                return;

//            Guard.NotNull(_sequenceProvider, nameof(_sequenceProvider));

//            int roundNumber = YG2.saves.GetRoundNumber();
//            Guard.Positive(roundNumber, nameof(roundNumber));

//            GameMode mode = YG2.saves.HasSavedMode
//                ? YG2.saves.LoadMode()
//                : _sequenceProvider.DetermineModeByRoundNumber(roundNumber);

//            ValidateMode(mode, nameof(mode));

//            DifficultyLevel difficulty = mode == GameMode.FreeMode
//                ? PickRandomDifficulty()
//                : GetBaseDifficulty(mode);

//            _status = BuildStatus(roundNumber, mode, difficulty);
//            _isInitialized = true;
//        }

//        public void SaveSelectedMode(GameMode mode)
//        {
//            ValidateMode(mode, nameof(mode));

//            if (CurrentMode == mode && YG2.saves.LoadMode() == mode)
//                return;

//            YG2.saves.SaveMode(mode);

//            DifficultyLevel difficulty = mode == GameMode.FreeMode
//                ? PickRandomDifficulty()
//                : GetBaseDifficulty(mode);

//            _status = BuildStatus(CurrentRoundNumber, mode, difficulty);
//        }

//        public static DifficultyLevel GetBaseDifficulty(GameMode mode)
//        {
//            ValidateMode(mode, nameof(mode));

//            return mode switch
//            {
//                GameMode.Easy => DifficultyLevel.Easy,
//                GameMode.Medium => DifficultyLevel.Medium,
//                GameMode.Hard => DifficultyLevel.MediumHard,
//                GameMode.Tutorial => DifficultyLevel.Easy,
//                _ => DifficultyLevel.Easy
//            };
//        }

//        public bool IsModeAvailable(GameMode mode)
//        {
//            ValidateMode(mode, nameof(mode));

//            return mode switch
//            {
//                GameMode.Tutorial => true,
//                GameMode.Easy => true,
//                GameMode.Medium => IsEasyCompleted,
//                GameMode.Hard => IsMediumCompleted,
//                GameMode.FreeMode => IsFreeModeAvailable,
//                _ => false
//            };
//        }

//        public bool IsLastRoundOfMode(GameMode mode, int roundNumber)
//        {
//            ValidateMode(mode, nameof(mode));

//            int end = _sequenceProvider.GetEndRoundNumber(mode);

//            return end > 0 && roundNumber == end;
//        }

//        private PlayerProgressStatus BuildStatus(
//            int roundNumber,
//            GameMode mode,
//            DifficultyLevel difficulty)
//        {
//            Guard.Positive(roundNumber, nameof(roundNumber));
//            ValidateMode(mode, nameof(mode));

//            return new PlayerProgressStatus(
//                roundNumber,
//                mode,
//                difficulty,
//                IsModeCompletedByRoundNumber(GameMode.Easy, roundNumber),
//                IsModeCompletedByRoundNumber(GameMode.Medium, roundNumber),
//                IsModeCompletedByRoundNumber(GameMode.Hard, roundNumber));
//        }

//        private bool IsModeCompletedByRoundNumber(GameMode mode, int roundNumber)
//        {
//            ValidateMode(mode, nameof(mode));
//            Guard.Positive(roundNumber, nameof(roundNumber));

//            int endRoundNumber = _sequenceProvider.GetEndRoundNumber(mode);

//            return endRoundNumber > 0 && roundNumber > endRoundNumber;
//        }

//        private static DifficultyLevel PickRandomDifficulty()
//        {
//            var levels = (DifficultyLevel[])Enum.GetValues(typeof(DifficultyLevel));

//            Guard.IsTrue(levels.Length > 0, nameof(levels),
//                $"В {nameof(DifficultyLevel)} нет ни одного значения.");

//            return levels[UnityEngine.Random.Range(0, levels.Length)];
//        }

//        private static void ValidateMode(GameMode mode, string argumentName)
//        {
//            Guard.IsTrue(
//                Enum.IsDefined(typeof(GameMode), mode),
//                argumentName,
//                $"Значение {mode} ({(int)mode}) не является допустимым {nameof(GameMode)}.");
//        }
//    }
//}