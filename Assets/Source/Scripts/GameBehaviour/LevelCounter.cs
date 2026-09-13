using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Extensions;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class LevelCounter : MonoBehaviour
    {
        private SequenceDifficultyLevel _currentLevel;

        public event Action<int> RoundChanged;

        public int RoundNumber { get; private set; }

        [Inject]
        public void Initialize(SequenceDifficultyLevel level)
        {
            Guard.NotNull(level, nameof(level));

            _currentLevel = level;
            RoundNumber = _currentLevel.RoundNumber;
            _currentLevel.RoundChanged += OnRoundChange;

            OnRoundChange(RoundNumber);
        }

        private void OnDisable()
        {
            _currentLevel.RoundChanged -= OnRoundChange;
        }

        private void OnRoundChange(int roundNumber)
        {
            Guard.NotNull(roundNumber, nameof(roundNumber));
            RoundNumber = roundNumber;
            RoundChanged?.Invoke(RoundNumber);
        }
    }
}
