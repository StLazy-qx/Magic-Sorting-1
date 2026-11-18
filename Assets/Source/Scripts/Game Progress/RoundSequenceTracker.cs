using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Player;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class RoundSequenceTracker : MonoBehaviour
    {
        private LevelCounter _levelCounter;
        private PlayerProgress _playerProgress;
        private bool _isCurrentRoundCompleted;

        public event Action<GameMode, int> RoundCompleted;
        public event Action<GameMode> ModeCompleted;
        public event Action RoundOpened;

        public bool IsInitialized => _levelCounter != null;
        public GameMode CurrentMode => _levelCounter.Mode;
        public int CurrentRoundIndex => _levelCounter.RoundIndex;
        public int CurrentRoundNumber => _levelCounter.RoundNumber;
        public bool IsLastRound => _levelCounter.IsLastRound;

        [Inject]
        private void Construct(PlayerProgress playerProgress)
        {
            _playerProgress = playerProgress;
        }

        public void Initialize(LevelCounter levelCounter)
        {
            Guard.NotNull(levelCounter, nameof(levelCounter));
            Unsubscribe();

            _levelCounter = levelCounter;
            _isCurrentRoundCompleted = false;
            _levelCounter.RoundChanged += OnRoundReopened;
            _levelCounter.RoundRestarted += OnRoundReopened;
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        public void CompleteRound(RoundRating rating)
        {
            Guard.IsTrue(IsInitialized, 
                "[RoundSequenceTracker] Initialize(levelCounter) ещё не вызывался.");
            Guard.IsTrue(rating != RoundRating.NotCompleted, nameof(rating),
                "Нельзя завершить раунд рейтингом NotCompleted.");

            if (_isCurrentRoundCompleted)
                return;

            _isCurrentRoundCompleted = true;

            GameMode mode = _levelCounter.Mode;
            int roundNumber = _levelCounter.RoundNumber;

            _playerProgress.MarkRoundFinished(mode, roundNumber, rating);
            RoundCompleted?.Invoke(mode, roundNumber);

            if (_levelCounter.IsLastRound)
                ModeCompleted?.Invoke(mode);
        }

        private void OnRoundReopened()
        {
            _isCurrentRoundCompleted = false;

            RoundOpened?.Invoke();
        }

        private void Unsubscribe()
        {
            if (_levelCounter == null)
                return;

            _levelCounter.RoundChanged -= OnRoundReopened;
            _levelCounter.RoundRestarted -= OnRoundReopened;
        }
    }
}