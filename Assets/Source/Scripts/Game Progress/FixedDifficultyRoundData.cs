using Assets.Source.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Source.Scripts.Game_Progress
{
    public sealed class FixedDifficultyRoundData : RoundData
    {
        public override DifficultyLevel Difficulty { get; }

        public FixedDifficultyRoundData(int roundIndex, DifficultyLevel difficulty, string id = null)
            : base(roundIndex, id) => Difficulty = difficulty;
    }
}
