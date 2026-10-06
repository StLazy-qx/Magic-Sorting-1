using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Extensions;
using System;
using UnityEngine;
using YG;
using Zenject;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class LevelCounter : MonoBehaviour
    {
        private SequenceDifficultyLevel _currentLevel;
		private int _roundNumber;
		
        public event Action RoundChanged;

        public int RoundNumber => _roundNumber;

        [Inject]
        public void Initialize(SequenceDifficultyLevel level)
        {
            Guard.NotNull(level, nameof(level));

            _currentLevel = level;
            _roundNumber = YG2.saves.GetRoundNumber();
            _currentLevel.RoundChanged += OnRoundChange;

            RoundChanged?.Invoke();
        }

        private void OnDisable()
        {
        	if (_currentLevel != null)
                _currentLevel.RoundChanged -= OnRoundChange;
        }

        private void OnRoundChange()
        {
        	_roundNumber++;
        
            YG2.saves.SaveRoundNumber(_roundNumber);
            RoundChanged?.Invoke();
        }
    }
}
