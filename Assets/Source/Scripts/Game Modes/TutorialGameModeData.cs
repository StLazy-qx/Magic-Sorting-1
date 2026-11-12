using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.Player;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Game_Modes
{
    public sealed class TutorialGameModeData : GameModeData
    {
        public override GameMode Mode => GameMode.Tutorial;
        public override bool IsEndless => false;
        public override IReadOnlyList<RoundData> Rounds { get; }

        public TutorialGameModeData()
            => Rounds = BuildRounds(
                DifficultyLevel.Easy,
                DifficultyLevel.MediumEasy,
                DifficultyLevel.Easy);
    }
}
