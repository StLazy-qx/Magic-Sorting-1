using UnityEngine;

namespace Assets.Source.Scripts.Game_Progress
{
    [CreateAssetMenu(menuName = "Game/Round Data", fileName = "Round_")]
    public class RoundData : ScriptableObject
    {
        [SerializeField, Min(1)] private int _roundIndex = 1;
        [SerializeField] private string _id;

        public int RoundIndex => _roundIndex;
        public string Id => _id;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_roundIndex < 1) _roundIndex = 1;
            if (_roundIndex > GameModeData.ExpectedRoundsCount)
                _roundIndex = GameModeData.ExpectedRoundsCount;
        }
#endif
    }
}