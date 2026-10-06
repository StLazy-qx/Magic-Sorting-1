using Assets.Source.Scripts.Enums;

namespace Assets.Source.Scripts.GameDifficulty
{
    class RandomDifficultyProvider
    {
        private float _easyProbability = 40f;
        private float _mediumEasyProbability = 25f;
        private float _mediumProbability = 20f;
        private float _mediumHardProbability = 10f;

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            IsInitialized = true;
        }

        public DifficultyLevel GetRandomDifficulty()
        {
            float randomValue = UnityEngine.Random.Range(0f, 100f);

            if (randomValue < _easyProbability)
                return DifficultyLevel.Easy;

            if (randomValue < _easyProbability + _mediumEasyProbability)
                return DifficultyLevel.MediumEasy;

            if (randomValue < _easyProbability +
                _mediumEasyProbability + _mediumProbability)
                return DifficultyLevel.Medium;

            if (randomValue < _easyProbability +
                _mediumEasyProbability + _mediumProbability +
                _mediumHardProbability)
                return DifficultyLevel.MediumHard;

            return DifficultyLevel.Hard;
        }
    }
}
