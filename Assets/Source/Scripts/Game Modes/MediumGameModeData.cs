using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.Player;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Game_Modes
{
    public sealed class MediumGameModeData : GameModeData
    {
        public override GameMode Mode => GameMode.Medium;
        public override bool IsEndless => false;
        public override IReadOnlyList<RoundData> Rounds { get; }

        public MediumGameModeData()
            => Rounds = BuildRounds(
                DifficultyLevel.Medium,
                DifficultyLevel.Medium,
                DifficultyLevel.Medium,
                DifficultyLevel.MediumHard,
                DifficultyLevel.MediumHard,
                DifficultyLevel.MediumHard);
    }
}
