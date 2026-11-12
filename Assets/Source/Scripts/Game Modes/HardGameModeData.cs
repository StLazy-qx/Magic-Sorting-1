using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.Player;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Game_Modes
{
    public sealed class HardGameModeData : GameModeData
    {
        public override GameMode Mode => GameMode.Hard;
        public override bool IsEndless => false;
        public override IReadOnlyList<RoundData> Rounds { get; }

        public HardGameModeData()
            => Rounds = BuildRounds(
                DifficultyLevel.MediumHard,
                DifficultyLevel.MediumHard,
                DifficultyLevel.MediumHard,
                DifficultyLevel.Hard,
                DifficultyLevel.Hard,
                DifficultyLevel.Hard);
    }
}
