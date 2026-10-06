using Assets.Source.Scripts.Enums;
using System;

namespace Assets.Source.Scripts.GameDifficulty
{
    public interface IDifficultySequence
    {
        public DifficultyLevel GetNext();

        public int RoundNumber { get; }

        public event Action<int> RoundChanged;
    }
}
