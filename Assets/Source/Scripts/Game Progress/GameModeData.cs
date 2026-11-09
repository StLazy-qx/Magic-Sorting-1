using Assets.Source.Scripts.Player;
using UnityEngine;

namespace Assets.Source.Scripts.Game_Progress
{
    [CreateAssetMenu(menuName = "Game/Game Mode Data", fileName = "Mode_")]
    public class GameModeData : ScriptableObject
    {
        public const int ExpectedRoundsCount = 6;

        [SerializeField] private GameMode _mode;
        [SerializeField] private RoundData[] _rounds = new RoundData[ExpectedRoundsCount];

        public GameMode Mode => _mode;
        public RoundData[] Rounds => _rounds;
        public int RoundsCount => _rounds?.Length ?? 0;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_rounds == null || _rounds.Length != ExpectedRoundsCount)
                _rounds = new RoundData[ExpectedRoundsCount];
        }
#endif
    }
}