using Assets.Source.Scripts.Enums;
using System.Collections.Generic;
using System;
using YG;

namespace Assets.Source.Scripts.EntryPoint
{
    public class SequenceDifficultyLevel : IObjectInitilizable
    {
        private const int InitialSequenceLength = 50;
        private const int ExtendBatchSize = 50;
        private const float EasyProbability = 40f;
        private const float MediumEasyProbability = 25f;
        private const float MediumProbability = 20f;
        private const float MediumHardProbability = 10f;

        private int _currentIndex;
        private int _roundNumber;
        private List<DifficultyLevel> _sequence = new();

        public event Action<int> RoundChanged;

        public int RoundNumber => _roundNumber;
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            _currentIndex = 0;
            _sequence = new List<DifficultyLevel>(InitialSequenceLength);

            ExtendSequence(InitialSequenceLength, _currentIndex);
            LoadRound();

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
            {
                ExtendSequence(ExtendBatchSize, _currentIndex);
            }

            DifficultyLevel level = _sequence[_currentIndex];
            _currentIndex++;
            _roundNumber++;

            SaveRound();

            return level;
        }
        
        public void ResetTutorialSequence()
        {
        	// отдельные переменные для индексов для нумерации туториала 
        	_currentIndex = 0;
        	_roundNumber = 0;
		}

        private void LoadRound()
        {
            _roundNumber = YG2.saves.GetRoundNumber();
            
            RoundChanged?.Invoke(_roundNumber);
        }

        private void SaveRound()
        {
            YG2.saves.SaveRoundNumber(_roundNumber);
            RoundChanged?.Invoke(_roundNumber);
        }

        private void ExtendSequence(int count, int beginNumber)
        {
            for (int i = beginNumber; i < beginNumber + count; i++)
            {
                _sequence.Add(GetRandomDifficulty());
            }
        }

        private DifficultyLevel GetRandomDifficulty()
        {
            float randomValue = UnityEngine.Random.Range(0f, 100f);

            if (randomValue < EasyProbability)
            {
                return DifficultyLevel.Easy;
            }

            if (randomValue < EasyProbability + MediumEasyProbability)
            {
                return DifficultyLevel.MediumEasy;
            }

            if (randomValue < EasyProbability +
                MediumEasyProbability + MediumProbability)
            {
                return DifficultyLevel.Medium;
            }

            if (randomValue < EasyProbability +
                MediumEasyProbability + MediumProbability +
                MediumHardProbability)
            {
                return DifficultyLevel.MediumHard;
            }

            return DifficultyLevel.Hard;
        }
    }
}