using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Player;
using System;
using UnityEngine;
using YG;
using Zenject;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class LevelCounter : MonoBehaviour
    {
        private const int FirstRound = 1;

        private SequenceDifficultyLevel _currentLevel;
        private PlayerProgress _playerProgress;
        private int _roundNumber;

        public event Action RoundChanged;
        public int RoundNumber => _roundNumber;

        [Inject]
        public void Initialize(SequenceDifficultyLevel level, PlayerProgress playerProgress)
        {
            Guard.NotNull(level, nameof(level));
            Guard.NotNull(playerProgress, nameof(playerProgress));

            _currentLevel = level;
            _playerProgress = playerProgress;
            _roundNumber = ResolveStartRoundNumber();
            _currentLevel.RoundChanged += OnRoundChange;

            RoundChanged?.Invoke();
        }

        private int ResolveStartRoundNumber()
        {
            // FreeMode единственный продолжает из сейва
            if (_playerProgress.CurrentMode == GameMode.FreeMode)
                return YG2.saves.GetRoundNumber();

            return FirstRound;
        }

        private void OnRoundChange()
        {
            _roundNumber++;

            // Глобальный сейв нужен только для endless
            if (_playerProgress.CurrentMode == GameMode.FreeMode)
                YG2.saves.SaveRoundNumber(_roundNumber);

            RoundChanged?.Invoke();
        }
    }

    //public class LevelCounter : MonoBehaviour
    //{
    //    private SequenceDifficultyLevel _currentLevel;
    //    private int _roundNumber;

    //    public event Action RoundChanged;

    //    public int RoundNumber => _roundNumber;

    //    [Inject]
    //    public void Initialize(SequenceDifficultyLevel level)
    //    {
    //        Guard.NotNull(level, nameof(level));

    //        _currentLevel = level;
    //        _roundNumber = YG2.saves.GetRoundNumber();
    //        _currentLevel.RoundChanged += OnRoundChange;

    //        RoundChanged?.Invoke();
    //    }

    //    private void OnDisable()
    //    {
    //        if (_currentLevel != null)
    //            _currentLevel.RoundChanged -= OnRoundChange;
    //    }

    //    private void OnRoundChange()
    //    {
    //        _roundNumber++;

    //        YG2.saves.SaveRoundNumber(_roundNumber);
    //        RoundChanged?.Invoke();
    //    }
    //}
}
