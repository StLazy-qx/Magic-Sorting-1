using Assets.Source.Scripts.UI.GameModeView;
using Assets.Source.Scripts.YG;
using System;

namespace Assets.Source.Scripts.ActionsHandlers
{
    public class ReverseRewardFlow
    {
        private readonly ClickModeTracker _modeTracker;
        private readonly RewardedadvertisingGateway _rewardedGateway;
        private readonly ReverseButtonView _reverseButtonView;
        private readonly string _rewardId;

        public event Action Ended;

        public ReverseRewardFlow(
            ClickModeTracker modeTracker,
            RewardedadvertisingGateway rewardedGateway,
            ReverseButtonView reverseButtonView,
            string rewardId)
        {
            _modeTracker = modeTracker;
            _rewardedGateway = rewardedGateway;
            _reverseButtonView = reverseButtonView;
            _rewardId = rewardId;
        }

        public void Show()
        {
            if (_rewardedGateway.IsShowing)
                return;

            _reverseButtonView.SetButtonInteractable(false);

            _rewardedGateway.ShowReward(
                _rewardId,
                onSuccess: OnSuccess,
                onError: OnError,
                onClose: OnClose);
        }

        private void OnSuccess()
        {
            _reverseButtonView.EnableButton();
            _modeTracker.ActivateReverseMode();
            _reverseButtonView.SetButtonInteractable(true);

            Ended?.Invoke();
        }

        private void OnError()
        {
            _reverseButtonView.SetButtonInteractable(true);
            _reverseButtonView.SetButtonLockedVisual(false);

            Ended?.Invoke();
        }

        private void OnClose()
        {
            _reverseButtonView.SetButtonInteractable(true);
            _reverseButtonView.SetButtonLockedVisual(true);

            Ended?.Invoke();
        }
    }
}
