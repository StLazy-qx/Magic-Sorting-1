using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.Player;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Game_Modes
{
    public sealed class EasyGameModeData : GameModeData
    {
        public override GameMode Mode => GameMode.Easy;
        public override bool IsEndless => false;
        public override IReadOnlyList<RoundData> Rounds { get; }

        public EasyGameModeData()
            => Rounds = BuildRounds(
                DifficultyLevel.Easy,
                DifficultyLevel.Easy,
                DifficultyLevel.Easy, 
                DifficultyLevel.MediumEasy,
                DifficultyLevel.MediumEasy,
                DifficultyLevel.MediumEasy);
    }
}
