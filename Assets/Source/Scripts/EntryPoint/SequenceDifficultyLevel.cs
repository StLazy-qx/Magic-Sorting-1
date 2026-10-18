using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.GameDifficulty;
using System;
using System.Collections.Generic;

namespace Assets.Source.Scripts.EntryPoint
{
    public class SequenceDifficultyLevel : IObjectInitilizable
    {
        private const int InitialSequenceLength = 50;
        private const int ExtendBatchSize = 50;
        private const int LengthDifficultyLevel = 6;

        private int _currentIndex;
        private RandomDifficultyProvider _randomDifficultyProvider;
        private List<DifficultyLevel> _sequence = new();

        public event Action RoundChanged;

        public bool IsInitialized { get; private set; }

        public IReadOnlyList<DifficultyLevel> Sequence => _sequence;

        public void Initialize()
        {
            _randomDifficultyProvider = new RandomDifficultyProvider();
            _randomDifficultyProvider.Initialize();

            _currentIndex = 0;

            CreateDifficultySequence(InitialSequenceLength);

            IsInitialized = true;
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
            if (_currentIndex >= _sequence.Count)
                ExtendSequence(ExtendBatchSize, _currentIndex);

            DifficultyLevel level = _sequence[_currentIndex];
            _currentIndex++;

            RoundChanged?.Invoke();

            return level;
        }

        private void CreateDifficultySequence(int count)
        {
            // Все уровни сложности из энама в порядке возрастания
            var levels = (DifficultyLevel[])Enum.GetValues(typeof(DifficultyLevel));
            Array.Sort(levels); // на случай, если порядок в энаме не совпадает с числовым значением

            _sequence = new List<DifficultyLevel>(count);

            for (int i = 0; i < count; i++)
            {
                int levelIndex = (i / LengthDifficultyLevel) % levels.Length;
                _sequence.Add(levels[levelIndex]);
            }
        }

        private void ExtendSequence(int count, int beginNumber)
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
    }
}