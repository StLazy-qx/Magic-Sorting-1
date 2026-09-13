using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.UI.GameModeView;
using Assets.Source.Scripts.YG;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.ActionsHandlers
{
    public class ClickModeSwitcher : MonoBehaviour
    {
        public string RewardID;

        private readonly ClickModeTracker _modeTracker = new ClickModeTracker();
        private ReverseButtonView _reverseButtonView;
        private ReverseRewardFlow _rewardFlow;

        public event Action<ClickImpactMode> ModeChanged
        {
            add => _modeTracker.ModeChanged += value;
            remove => _modeTracker.ModeChanged -= value;
        }

        public event Action ReverseButtonActivating
        {
            add => _modeTracker.ReverseButtonActivated += value;
            remove => _modeTracker.ReverseButtonActivated -= value;
        }

        public event Action RewardedEnded;

        public ClickImpactMode CurrentMode => _modeTracker.CurrentMode;

        private void Awake()
        {
            _modeTracker.ActivateDistributionMode();
        }

        public void Reset()
        {
            Guard.NotNull(_reverseButtonView, nameof(_reverseButtonView));
            _modeTracker.Reset();
            _reverseButtonView.ResetState();
            UpdateButtonState();
        }

        public void SetButton(ReverseButtonView reverseButtonView)
        {
            Guard.NotNull(reverseButtonView, nameof(reverseButtonView));
            Guard.NotNullOrWhiteSpace(RewardID, nameof(RewardID));

            _reverseButtonView = reverseButtonView;
            _rewardFlow = new ReverseRewardFlow(
                _modeTracker,
                new RewardedadvertisingGateway(),
                _reverseButtonView,
                RewardID);

            _rewardFlow.Ended += () => RewardedEnded?.Invoke();
            _reverseButtonView.ButtonClicked += OnToggleMode;
        }

        public void Reverse()
        {
            Guard.NotNull(_reverseButtonView, nameof(_reverseButtonView));

            if (CurrentMode != ClickImpactMode.ModeReverse)
                return;

            _modeTracker.MarkReverseUsed();
            UpdateButtonState();
        }

        public void OnToggleMode()
        {
            Guard.NotNull(_reverseButtonView, nameof(_reverseButtonView));

            if (CurrentMode == ClickImpactMode.ModeReverse)
            {
                _modeTracker.ActivateDistributionMode();

                return;
            }

            if (_modeTracker.IsReverseUsed)
            {
                if (_reverseButtonView.IsRewardedIconActive == false)
                {
                    _modeTracker.ActivateReverseMode();

                    return;
                }

                Guard.NotNull(_rewardFlow, nameof(_rewardFlow));
                _rewardFlow.Show();
            }
            else
            {
                _modeTracker.ActivateReverseMode();
            }
        }

        private void UpdateButtonState()
        {
            Guard.NotNull(_reverseButtonView, nameof(_reverseButtonView));
            _reverseButtonView.SetReverseUsedState(_modeTracker.IsReverseUsed);
        }
    }
}
