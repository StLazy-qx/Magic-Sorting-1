using Assets.Source.Scripts.Enums;
using System;
using Zenject;

namespace Assets.Source.Scripts.GameDifficulty
{
    public class DifficultyState : IInitializable
    {
        public DifficultyLevel CurrentDifficulty { get; private set; } = DifficultyLevel.Easy;

        public event Action<DifficultyLevel> DifficultyChanged;

        public void SetDifficulty(DifficultyLevel difficultyLevel)
        {
            if (CurrentDifficulty == difficultyLevel)
                return;

            CurrentDifficulty = difficultyLevel;

            DifficultyChanged?.Invoke(difficultyLevel);
        }

        public void Initialize() { }
    }
}