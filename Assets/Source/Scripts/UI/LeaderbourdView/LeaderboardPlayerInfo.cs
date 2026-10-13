using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using YG;
using YG.Utils.LB;

namespace Assets.Source.Scripts.UI.LeaderbourdView
{
    public class LeaderboardPlayerInfo : MonoBehaviour
    {
        private const string leaderboardName = "GameLeaderboard";

        [SerializeField] private TMP_Text _rankText;
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _scoreText;

        private void OnEnable()
        {
            YG2.onGetLeaderboard += OnLeaderboardReceived;

            RefreshFromCurrentPlayer();
            YG2.GetLeaderboard(leaderboardName);
        }

        private void OnDisable()
        {
            YG2.onGetLeaderboard -= OnLeaderboardReceived;
        }

        public void RefreshFromCurrentPlayer()
        {
            var player = YG2.player;

            if (player == null)
                return;

            _nameText.text = string.IsNullOrEmpty(player.name)
                ? "Аноним"
                : player.name;

            _scoreText.text = YG2.saves.Points.ToString();

            LoadAvatar(player.photo);
        }

        private void OnLeaderboardReceived(LBData data)
        {
            if (data == null || data.technoName != leaderboardName)
                return;

            int rank = data.currentPlayer != null
                ? data.currentPlayer.rank
                : 0;

            _rankText.text = rank > 0
                ? rank.ToString()
                : "-";
        }

        private void LoadAvatar(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                _avatarImage.sprite = null;
                _avatarImage.color = new Color(1, 1, 1, 0);

                return;
            }

            _avatarImage.color = Color.white;

            StartCoroutine(DownloadAvatar(url));
        }

        private IEnumerator DownloadAvatar(string url)
        {
            using (UnityWebRequest request =
                   UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning(
                        "Не удалось загрузить аватар: " + request.error);

                    yield break;
                }

                Texture2D texture =
                    DownloadHandlerTexture.GetContent(request);

                if (texture == null)
                    yield break;

                _avatarImage.sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
            }
        }
    }
}
