using Assets.Source.Scripts.UI.LeaderbourdView;
using Assets.Source.Scripts.Extensions;
using UnityEngine;
using YG.Utils.LB;
using YG;

namespace Assets.Source.Scripts.Leaderboard
{
    class LeaderboardSetter : MonoBehaviour
    {
        [SerializeField] private LeaderbourdCheckEmptyText _checkEmptyText;
        [SerializeField] private LeaderboardYG _leaderboardYG;
        [SerializeField] private Leaderboard _leaderboard;
        [SerializeField] private LeaderboardPlayerInfo _playerInfo;

        private void Awake()
        {
            Guard.NotNull(_checkEmptyText, nameof(_checkEmptyText));
            Guard.NotNull(_leaderboardYG, nameof(_leaderboardYG));
            Guard.NotNull(_leaderboard, nameof(_leaderboard));
            Guard.NotNull(_playerInfo, nameof(_playerInfo));
        }

        private void OnEnable()
        {
            YG2.onGetLeaderboard += OnGetLeaderboard;
        }

        private void OnDisable()
        {
            YG2.onGetLeaderboard -= OnGetLeaderboard;
        }

        private void Start()
        {
            RequestLeaderboard();
        }

        private void RequestLeaderboard()
        {
            int checkTopQuantity = 1;
            int checkAroundQuantity = 4;

            if (string.IsNullOrEmpty(_leaderboardYG.nameLB) == false)
            {
                YG2.GetLeaderboard(_leaderboardYG.nameLB,
                    checkTopQuantity, checkAroundQuantity, "small");
            }
        }

        private void OnGetLeaderboard(LBData data)
        {
            if (data.technoName != _leaderboardYG.nameLB)
                return;

            bool hasPlayers = data.players != null
                && data.players.Length > 0;

            if (hasPlayers)
            {
                _leaderboard.gameObject.SetActive(true);
                _checkEmptyText.HideText();
            }
            else
            {
                _leaderboard.gameObject.SetActive(false);
                _checkEmptyText.ShowText();
            }

            _playerInfo.RefreshFromCurrentPlayer();
        }
    }
}