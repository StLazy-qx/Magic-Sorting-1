using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.Player;
using System;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Game_Modes
{
    public sealed class FreeGameModeData : GameModeData
    {
        private const int InitialBatchSize = 50;
        private const int DifficultyStep = 6;

        public override GameMode Mode => GameMode.FreeMode;
        public override bool IsEndless => true;
        public override IReadOnlyList<RoundData> Rounds { get; }

        public FreeGameModeData()
        {
            var levels = (DifficultyLevel[])Enum.GetValues(typeof(DifficultyLevel));
            Array.Sort(levels);

            var rounds = new RoundData[InitialBatchSize];

            for (int i = 0; i < InitialBatchSize; i++)
            {
                int levelIndex = (i / DifficultyStep) % levels.Length;
                rounds[i] = new FixedDifficultyRoundData(i + 1, levels[levelIndex]);
            }

            Rounds = rounds;
        }
    }
}
