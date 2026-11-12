using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Player;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Game_Progress
{
    public abstract class GameModeData
    {
        public const int ExpectedRoundsCount = 6;

        public abstract GameMode Mode { get; }
        public abstract bool IsEndless { get; }
        public abstract IReadOnlyList<RoundData> Rounds { get; }
        public int RoundsCount => Rounds.Count;

        protected static RoundData[] BuildRounds(params DifficultyLevel[] difficulties)
        {
            var rounds = new RoundData[difficulties.Length];

            for (int i = 0; i < difficulties.Length; i++)
                rounds[i] = new FixedDifficultyRoundData(i + 1, difficulties[i]);

            return rounds;
        }
    }

    //    [CreateAssetMenu(menuName = "Game/Game Mode Data", fileName = "Mode_")]
    //    public class GameModeData : ScriptableObject
    //    {
    //        public const int ExpectedRoundsCount = 6;

    //        [SerializeField] private GameMode _mode;
    //        [SerializeField] private RoundData[] _rounds = new RoundData[ExpectedRoundsCount];

    //        public GameMode Mode => _mode;
    //        public RoundData[] Rounds => _rounds;
    //        public int RoundsCount => _rounds?.Length ?? 0;

    //#if UNITY_EDITOR
    //        private void OnValidate()
    //        {
    //            if (_rounds == null || _rounds.Length != ExpectedRoundsCount)
    //                _rounds = new RoundData[ExpectedRoundsCount];
    //        }
    //#endif
    //    }
}