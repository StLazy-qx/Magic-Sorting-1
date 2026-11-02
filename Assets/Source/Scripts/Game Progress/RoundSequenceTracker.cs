using System;
using System.Collections.Generic;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.YG;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class RoundSequenceTracker : IDisposable
    {
        private const int FreeModeExtensionBatchSize = 50;

        private readonly List<int> _roundsSequence;
        private readonly PlayerProgress _playerProgress;
        private readonly RoundRatingSection _ratingSection;
        private readonly RoundSequenceProvider _sequenceProvider;
        private readonly bool _isEndlessMode;

        private int _currentRoundIndex;
        private int _maxRoundsInCurrentMode;

        public event Action<int> CurrentRoundIndexChanged;
        public event Action<GameMode, int> RoundCompleted;
        public event Action<GameMode> ModeCompleted;

        public RoundSequenceTracker(
            IReadOnlyList<int> roundsSequence,
            PlayerProgress playerProgress,
            RoundRatingSection ratingSection,
            RoundSequenceProvider sequenceProvider)
        {
            Guard.NotNullOrEmpty(roundsSequence, nameof(roundsSequence));
            Guard.NotNull(playerProgress, nameof(playerProgress));
            Guard.NotNull(ratingSection, nameof(ratingSection));
            Guard.NotNull(sequenceProvider, nameof(sequenceProvider));

            _roundsSequence = new List<int>(roundsSequence);
            _playerProgress = playerProgress;
            _ratingSection = ratingSection;
            _sequenceProvider = sequenceProvider;

            _isEndlessMode = playerProgress.CurrentMode == GameMode.FreeMode;

            _maxRoundsInCurrentMode = _roundsSequence.Count;
            _currentRoundIndex = ResolveCurrentRoundIndexFromProgress();
        }

        public int CurrentRoundIndex => _currentRoundIndex;
        public int MaxRoundsInCurrentMode => _maxRoundsInCurrentMode;
        public GameMode CurrentMode => _playerProgress.CurrentMode;
        public bool IsEndlessMode => _isEndlessMode;

        public bool IsModeCompleted
            => !_isEndlessMode && _currentRoundIndex >= _maxRoundsInCurrentMode;

        public int CurrentRoundNumber
            => _currentRoundIndex >= 0 && _currentRoundIndex < _roundsSequence.Count
                ? _roundsSequence[_currentRoundIndex]
                : _roundsSequence[_roundsSequence.Count - 1];

        public void SetCurrentRoundIndex(int index)
        {
            Guard.InRange(index, 0, _maxRoundsInCurrentMode, nameof(index));

            if (_currentRoundIndex == index)
                return;

            _currentRoundIndex = index;
            CurrentRoundIndexChanged?.Invoke(_currentRoundIndex);
        }

        public void SetMaxRoundNumber(int maxRounds)
        {
            Guard.Positive(maxRounds, nameof(maxRounds));
            _maxRoundsInCurrentMode = maxRounds;
        }

        public void OnRoundCompleted(RoundRating rating)
        {
            if (IsModeCompleted)
                return;

            if (rating == RoundRating.NotCompleted)
                return;

            int roundNumber = CurrentRoundNumber;

            _ratingSection.Save(CurrentMode, roundNumber, rating);
            RoundCompleted?.Invoke(CurrentMode, roundNumber);

            _currentRoundIndex++;

            if (_isEndlessMode)
                EnsureNextRoundAvailable();

            CurrentRoundIndexChanged?.Invoke(_currentRoundIndex);

            if (_isEndlessMode == false &&
                _currentRoundIndex == _maxRoundsInCurrentMode)
            {
                ModeCompleted?.Invoke(CurrentMode);
            }
        }

        public void Dispose()
        {
            CurrentRoundIndexChanged = null;
            RoundCompleted = null;
            ModeCompleted = null;
        }

        private void EnsureNextRoundAvailable()
        {
            if (_currentRoundIndex < _roundsSequence.Count)
                return;

            int lastRoundNumber = _roundsSequence[_roundsSequence.Count - 1];

            IReadOnlyList<int> extension = _sequenceProvider.GetFreeMode(
                lastRoundNumber + 1,
                FreeModeExtensionBatchSize);

            _roundsSequence.AddRange(extension);
            _maxRoundsInCurrentMode = _roundsSequence.Count;
        }

        private int ResolveCurrentRoundIndexFromProgress()
        {
            int savedRoundNumber = _playerProgress.CurrentRoundNumber;

            for (int index = 0; index < _roundsSequence.Count; index++)
            {
                if (_roundsSequence[index] == savedRoundNumber)
                    return index;
            }

            return 0;
        }
    }
}