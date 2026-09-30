using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using System;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Tutorial
{
    public sealed class TutorialProgress
    {
        private readonly SequenceDifficultyLevel _sequenceDifficultyLevel;

        private int _currentIndex;
        private bool _isCompleted;
        private IReadOnlyList<DifficultyLevel> _sequence;

        public event Action<int> RoundStarted;
        public event Action Completed;

        public bool IsFinishRound => _sequence != null
            && _currentIndex >= _sequence.Count;

        public TutorialProgress(SequenceDifficultyLevel sequenceDifficultyLevel)
        {
            Guard.NotNull(sequenceDifficultyLevel, nameof(sequenceDifficultyLevel));

            _sequenceDifficultyLevel = sequenceDifficultyLevel;
        }

        public void Reset()
        {
            _sequence = _sequenceDifficultyLevel.GetTutorialSequence();
            _currentIndex = 0;
            _isCompleted = false;
        }

        public bool TryMoveNext(out DifficultyLevel level)
        {
            level = default;

            if (_isCompleted)
                return false;

            if (_sequence == null || _currentIndex >= _sequence.Count)
            {
                _isCompleted = true;

                Completed?.Invoke();

                return false;
            }

            level = _sequence[_currentIndex];
            _currentIndex++;

            RoundStarted?.Invoke(_currentIndex);

            return true;
        }
    }
}
