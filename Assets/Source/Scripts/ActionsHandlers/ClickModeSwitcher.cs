using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.UI.GameModeView;
using Assets.Source.Scripts.UI.Buttons;
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
        private ButtonColorMixer _buttonColorMixer;
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
            _buttonColorMixer?.ResetState();
            UpdateButtonState();
        }

        public void SetModeButtons(
            ReverseButtonView reverseButtonView,
            ButtonColorMixer buttonColorMixer)
        {
            Guard.NotNull(reverseButtonView, nameof(reverseButtonView));
            Guard.NotNullOrWhiteSpace(RewardID, nameof(RewardID));
            Guard.NotNull(buttonColorMixer, nameof(buttonColorMixer));

            if (_buttonColorMixer != null)
                _buttonColorMixer.OnClick.RemoveListener(OnToggleColorMixerMode);

            if (_reverseButtonView != null)
                _reverseButtonView.ButtonClicked -= OnToggleMode;

            _reverseButtonView = reverseButtonView;
            _buttonColorMixer = buttonColorMixer;
            _rewardFlow = new ReverseRewardFlow(
                _modeTracker,
                new RewardedadvertisingGateway(),
                _reverseButtonView,
                RewardID);
            _rewardFlow.Ended += () => RewardedEnded?.Invoke();
            _reverseButtonView.ButtonClicked += OnToggleMode;
            _buttonColorMixer.OnClick.AddListener(OnToggleColorMixerMode);
            _modeTracker.ReverseButtonActivated +=
                _reverseButtonView.ActivateRewardedIcon;
            _modeTracker.ModeChanged += OnModeChanged;
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

            if (CurrentMode == ClickImpactMode.ModeColorMixing)
            {
                _modeTracker.ActivateDistributionMode();
                
                return;
            }

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

        public void OnToggleColorMixerMode()
        {
            Guard.NotNull(_buttonColorMixer, nameof(_buttonColorMixer));

            if (CurrentMode == ClickImpactMode.ModeColorMixing)
            {
                _modeTracker.ActivateDistributionMode();
                
                return;
            }

            _modeTracker.ActivateMixerColorMode();
        }

        public void SetButtonForTutorial(ReverseButtonView reverseButtonView)
        {
            Guard.NotNull(reverseButtonView, nameof(reverseButtonView));

            _reverseButtonView = reverseButtonView;
            _reverseButtonView.HideRewardedIconPermanently();
            _reverseButtonView.ButtonClicked += OnToggleModeTutorial;
        }

        public void OnToggleModeTutorial()
        {
            Guard.NotNull(_reverseButtonView, nameof(_reverseButtonView));

            if (CurrentMode == ClickImpactMode.ModeReverse)
            {
                _modeTracker.ActivateDistributionMode();
                
                return;
            }

            _modeTracker.ActivateReverseMode();
        }

        private void OnModeChanged(ClickImpactMode mode)
        {
            if (_buttonColorMixer == null)
                return;

            if (mode == ClickImpactMode.ModeColorMixing)
                _buttonColorMixer.Disable();
            else
                _buttonColorMixer.Enable();
        }

        private void UpdateButtonState()
        {
            Guard.NotNull(_reverseButtonView, nameof(_reverseButtonView));
            _reverseButtonView.SetReverseUsedState(_modeTracker.IsReverseUsed);
        }
    }
}