using Assets.Source.Scripts.UI.Buttons;
using UnityEngine;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class RewardedAdvertising : MonoBehaviour
    {
        [SerializeField] private IconRewardedAdvertisement _iconRewardedD;
        [SerializeField] private IconRewardedAdvertisement _iconRewardedM;

        public string rewardID;

        //инициализация кнопки через EntryPoint
        private void OnEnable()
        {
            //_buttonRewardedD.OnClick.AddListener(OnGetLog);
            //_buttonRewardedM.OnClick.AddListener(OnGetLog);

            //YG2.onCloseRewardedAdv += OnCloseButtonRewardedAdv;
        }

        private void OnDisable()
        {
            //_buttonRewardedD.OnClick.RemoveListener(OnGetLog);
            //_buttonRewardedM.OnClick.RemoveListener(OnGetLog);

            //YG2.onCloseRewardedAdv -= OnCloseButtonRewardedAdv;
        }

        private void OnGetLog()
        {
            //YG2.InterstitialAdvShow();

            YG2.RewardedAdvShow(rewardID, () =>
            {
                Debug.Log("Вызвана реклама за вознаграждения");
            });
        }

        private void OnCloseButtonRewardedAdv()
        {
            _iconRewardedD.Disable();
            _iconRewardedM.Disable();
        }
    }
}
