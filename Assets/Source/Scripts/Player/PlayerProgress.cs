using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Game_Modes;
using Assets.Source.Scripts.Game_Progress;
using System;
using UnityEngine;
using YG;
using Zenject;

namespace Assets.Source.Scripts.Player
{
    public class PlayerProgress : IInitializable
    {
        private const int FirstRoundNumber = 1;

        private bool _isInitialized;
        private int _roundNumber = FirstRoundNumber;
        private IGameModeDataProvider _dataProvider;
        private GameMode _currentMode = GameMode.Tutorial;

        public bool IsInitialized => _isInitialized;
        public bool IsFreeModeAvailable => IsModeAvailable(GameMode.FreeMode);
        public int CurrentRoundNumber => _roundNumber;
        public GameMode CurrentMode => _currentMode;

        //[Obsolete("Временная заглушка этапа 1, удалить на этапе 2")]
        //public DifficultyLevel CurrentDifficultyLevel
        //    => GetModeData(_currentMode).GetDifficulty(_currentMode == GameMode.FreeMode ? _roundNumber - 1 : 0);

        [Inject]
        private void Construct(IGameModeDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public void Initialize()
        {
            if (_isInitialized)
                return;

            Reload();
        }

        public void Reload()
        {
            _currentMode = YG2.saves.GameMode.HasSavedMode
                ? YG2.saves.GameMode.Load()
                : GameMode.Tutorial;

            _roundNumber = ResolveRoundNumber(_dataProvider.Get(_currentMode));
            _isInitialized = true;
        }

        public void SaveSelectedMode(GameMode mode)
        {
            YG2.saves.GameMode.Save(mode);
            Reload();
        }

        public bool IsModeAvailable(GameMode mode)
        {
            foreach (GameMode required in _dataProvider.Get(mode).RequiredModes)
            {
                if (IsModeCompleted(required) == false)
                    return false;
            }

            return true;
        }

        public GameModeData GetModeData(GameMode mode)
            => _dataProvider.Get(mode);

        public int GetTotalRoundsCount(GameMode mode)
        {
            GameModeData data = _dataProvider.Get(mode);

            return data.IsEndless ? 0 : data.RoundsCount;
        }

        public void MarkRoundFinished(GameMode mode, int roundNumber, RoundRating rating, bool keepBest = true)
        {
            Guard.IsTrue(rating != RoundRating.NotCompleted, nameof(rating),
                "Нельзя закончить раунд рейтингом NotCompleted.");

            GameModeData data = _dataProvider.Get(mode);

            if (data.IsRated == false)
                return;

            Guard.InRange(roundNumber, 1, data.RoundsCount, nameof(roundNumber));
            YG2.saves.RoundRatings.Save(mode, roundNumber, rating, keepBest);
        }

        public RoundRating GetRoundRating(GameMode mode, int roundNumber)
            => YG2.saves.RoundRatings.Get(mode, roundNumber);

        public int GetCompletedRoundsCount(GameMode mode)
        {
            if (_dataProvider.Get(mode).IsRated == false)
                return 0;

            int total = GetTotalRoundsCount(mode);
            int count = 0;

            for (int roundNumber = 1; roundNumber <= total; roundNumber++)
            {
                if (GetRoundRating(mode, roundNumber) != RoundRating.NotCompleted)
                    count++;
            }

            return count;
        }

        public bool IsModeCompleted(GameMode mode)
        {
            int total = GetTotalRoundsCount(mode);

            return _dataProvider.Get(mode).IsRated
                && total > 0
                && GetCompletedRoundsCount(mode) == total;
        }

        public bool TryGetModeRating(GameMode mode, out RoundRating rating)
        {
            rating = RoundRating.NotCompleted;

            if (IsModeCompleted(mode) == false)
                return false;

            int total = GetTotalRoundsCount(mode);
            int sum = 0;

            for (int roundNumber = 1; roundNumber <= total; roundNumber++)
            {
                sum += (int)GetRoundRating(mode, roundNumber);
            }

            int rounded = (int)Math.Round((double)sum / total, MidpointRounding.AwayFromZero);
            rating = (RoundRating)Mathf.Clamp(rounded, (int)RoundRating.Bad, (int)RoundRating.Good);

            return true;
        }

        /// <summary>Только endless продолжает из сейва, остальные начинаются с первого раунда.</summary>
        private static int ResolveRoundNumber(GameModeData data)
        {
            return data.IsEndless
                ? Mathf.Max(FirstRoundNumber, YG2.saves.Round.GetNumber())
                : FirstRoundNumber;
        }
    }
}