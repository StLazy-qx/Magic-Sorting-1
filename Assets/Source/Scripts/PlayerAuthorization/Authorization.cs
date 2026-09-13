using UnityEngine;
using YG;

namespace Assets.Source.Scripts.PlayerAuthorization
{
    public class Authorization : MonoBehaviour
    {
        private string _playerID;

        private void Awake()
        {
            _playerID = YG2.player.id;
        }

        private void OnEnable()
        {
            YG2.onGetSDKData += UpdatePlayerData;
        }

        private void OnDisable()
        {
            YG2.onGetSDKData -= UpdatePlayerData;
        }

        private void AuthorizePlayer()
        {
            if (YG2.player.auth)
            {
                UpdatePlayerData();
                return;
            }

            YG2.OpenAuthDialog();
        }

        private void UpdatePlayerData()
        {
            if (YG2.player.auth)
            {
                _playerID = YG2.player.id;
                Debug.Log($"Игрок авторизован. ID: {_playerID}, Ник: {YG2.player.name}");
            }
            else
            {
                _playerID = "unauthorized";
                Debug.Log("Игрок не авторизован.");
            }
        }
    }
}