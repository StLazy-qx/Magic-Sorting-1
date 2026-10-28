using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.EntryPoint
{
    public class SequenceDifficultyLevel : IObjectInitilizable
    {
        private const int ModeRoundsLength = 6;
        private const int FreeModeInitialLength = 50;
        private const int FreeModeExtendBatchSize = 50;
        private const int LengthDifficultyLevel = 6;

        private readonly List<DifficultyLevel> _sequence = new();

        private bool _isFixedMode;
        private int _currentIndex;
        private RandomDifficultyProvider _randomDifficultyProvider;
        private PlayerProgress _playerProgress;
        private GameMode _gameMode = GameMode.FreeMode;
        private GameMode? _builtForMode;

        public event Action RoundChanged;
        public bool IsInitialized { get; private set; }
        public int RoundsInSeries => ModeRoundsLength;

        [Inject]
        private void Construct(PlayerProgress playerProgress)
        {
            _playerProgress = playerProgress;
        }

        public void Initialize()
        {
            _gameMode = GetCurrentMode();
            _builtForMode = _gameMode;
            _currentIndex = 0;

            _sequence.Clear();

            if (_gameMode == GameMode.FreeMode)
            {
                _isFixedMode = false;
                _randomDifficultyProvider = new RandomDifficultyProvider();

                _randomDifficultyProvider.Initialize();
                CreateFreeModeSequence(FreeModeInitialLength, 0);
            }
            else
            {
                _isFixedMode = true;
                _sequence.AddRange(BuildFixedPlan(_gameMode));
            }

            IsInitialized = true;
        }

        public void SyncWithProgress()
        {
            GameMode current = GetCurrentMode();

            if (IsInitialized == false || _builtForMode != current)
                Initialize();
        }

        public IReadOnlyList<DifficultyLevel> GetTutorialSequence()
        {
            return new List<DifficultyLevel>
            {
                DifficultyLevel.Easy,
                DifficultyLevel.MediumEasy
            }.AsReadOnly();
        }

        public DifficultyLevel GetNext()
        {
            if (IsInitialized == false)
                Initialize();

            if (_currentIndex >= _sequence.Count)
            {
                if (_isFixedMode)
                    ExtendFixedSequence();
                else
                    CreateFreeModeSequence(FreeModeExtendBatchSize, _currentIndex);
            }

            DifficultyLevel level = _sequence[_currentIndex];
            _currentIndex++;

            RoundChanged?.Invoke();

            return level;
        }

        private GameMode GetCurrentMode()
        {
            return _playerProgress != null
                ? _playerProgress.CurrentMode
                : GameMode.FreeMode;
        }

        private static DifficultyLevel[] BuildFixedPlan(GameMode mode)
        {
            DifficultyLevel level = PlayerProgress.GetBaseDifficulty(mode);
            DifficultyLevel[] plan = new DifficultyLevel[ModeRoundsLength];

            for (int i = 0; i < plan.Length; i++)
                plan[i] = level;

            return plan;
        }

        private void CreateFreeModeSequence(int count, int beginNumber)
        {
            var levels = (DifficultyLevel[])Enum.GetValues(typeof(DifficultyLevel));

            Array.Sort(levels);

            int end = beginNumber + count;

            for (int i = beginNumber; i < end; i++)
            {
                int levelIndex = (i / LengthDifficultyLevel) % levels.Length;

                _sequence.Add(levels[levelIndex]);
            }
        }

        private void ExtendFixedSequence()
        {
            _sequence.AddRange(BuildFixedPlan(_gameMode));
        }
    }
}