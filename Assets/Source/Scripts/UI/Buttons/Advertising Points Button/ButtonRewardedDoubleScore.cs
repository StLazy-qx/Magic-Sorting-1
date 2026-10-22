using System;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Zenject;
using Assets.Source.Scripts.Player;

namespace Assets.Source.Scripts.UI.Buttons.Advertising_Points_Button
{
    [RequireComponent(typeof(Button))]
    public class ButtonRewardedDoubleScore : MonoBehaviour
    {
        [SerializeField] private string _rewardId = "double_score";

        private Button _button;
        private Wallet _wallet;
        private bool _isShowing;
        private bool _isRewarded;

        [Inject]
        private void Construct(Wallet wallet)
        {
            _wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);

            YG2.onCloseRewardedAdv += OnAdFinished;
            YG2.onErrorRewardedAdv += OnAdFinished;
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);

            YG2.onCloseRewardedAdv -= OnAdFinished;
            YG2.onErrorRewardedAdv -= OnAdFinished;
        }

        private void OnButtonClick()
        {
            if (_isShowing)
                return;

            _isShowing = true;
            _isRewarded = false;
            _button.interactable = false;

            YG2.RewardedAdvShow(_rewardId, OnRewarded);
        }

        // Вызывается только когда реклама досмотрена до конца
        private void OnRewarded()
        {
            if (_isRewarded)
                return;

            _isRewarded = true;

            // Wallet сам вызовет события, UI обновится через подписчиков
            _wallet.MultiplyLastRoundScore();
        }

        // Реклама закрыта или произошла ошибка
        private void OnAdFinished()
        {
            _isShowing = false;

            // Если награды не было (закрыли раньше или ошибка), даём попробовать снова
            _button.interactable = !_isRewarded;
        }
    }
}
