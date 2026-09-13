using Assets.Source.Scripts.Extensions;
using System;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class RewardedadvertisingGateway
    {
        public bool IsShowing { get; private set; }

        private Action _currentOnError;
        private Action _currentOnClose;

        public void ShowReward(
            string rewardId, 
            Action onSuccess,
            Action onError, 
            Action onClose)
        {
            Guard.NotNullOrWhiteSpace(rewardId, nameof(rewardId));
            Guard.NotNull(onSuccess, nameof(onSuccess));

            if (IsShowing)
                return;

            IsShowing = true;

            SetupCallbacks(onError, onClose);
            Subscribe(_currentOnError, _currentOnClose);
            YG2.RewardedAdvShow(rewardId, () => CompleteReward(onSuccess));
        }

        private void CompleteReward(Action callback)
        {
            Unsubscribe();

            IsShowing = false;

            callback?.Invoke();
        }

        private void SetupCallbacks(Action onError, Action onClose)
        {
            _currentOnError = () => CompleteReward(onError);
            _currentOnClose = () => CompleteReward(onClose);
        }

        private void Subscribe(Action onError, Action onClose)
        {
            YG2.onErrorRewardedAdv += onError;
            YG2.onCloseRewardedAdv += onClose;
        }

        private void Unsubscribe()
        {
            if (_currentOnError != null)
            {
                YG2.onErrorRewardedAdv -= _currentOnError;
                _currentOnError = null;
            }

            if (_currentOnClose != null)
            {
                YG2.onCloseRewardedAdv -= _currentOnClose;
                _currentOnClose = null;
            }
        }
    }
}
