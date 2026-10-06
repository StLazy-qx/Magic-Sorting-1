using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.GameDifficulty;
using System.Collections.Generic;
using System;

namespace Assets.Source.Scripts.EntryPoint
{
    public class SequenceDifficultyLevel : IObjectInitilizable
    {   
        private const int InitialSequenceLength = 50;
        private const int ExtendBatchSize = 50;

        private int _currentIndex;
        private RandomDifficultyProvider _randomDifficultyProvider;
        private List<DifficultyLevel> _sequence = new();

        public event Action RoundChanged;

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
        	_randomDifficultyProvider = new RandomDifficultyProvider();
        
        	_randomDifficultyProvider.Initialize();
        
            _currentIndex = 0;
            _sequence = new List<DifficultyLevel>(InitialSequenceLength);

            ExtendSequence(InitialSequenceLength, _currentIndex);
            
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

        private void ExtendSequence(int count, int beginNumber)
        {
            for (int i = beginNumber; i < beginNumber + count; i++)
            {
                _sequence.Add(_randomDifficultyProvider.GetRandomDifficulty());
            }
        }
    }
}