using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.UI.GameModeView;
using Assets.Source.Scripts.Color_Mixing_Mechanics;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.YG;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.ActionsHandlers
{
    public class ClickModeSwitcher : MonoBehaviour
    {
        [SerializeField] private VesselMixer _vesselMixer;

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
        public event Action<MagicCell, MagicCell> ColorsCannotBeMixed;

        public ClickImpactMode CurrentMode => _modeTracker.CurrentMode;

        private void Awake()
        {
            Guard.NotNull(_vesselMixer, nameof(_vesselMixer));
            _modeTracker.ActivateDistributionMode();
        }

        public void Reset()
        {
            Guard.NotNull(_reverseButtonView, nameof(_reverseButtonView));
            _modeTracker.Reset();
            _reverseButtonView.ResetState();
            _vesselMixer?.Deactivate();
            UpdateButtonState();
        }

        public void SetModeButtons(ReverseButtonView reverseButtonView)
        {
            Guard.NotNull(reverseButtonView, nameof(reverseButtonView));
            Guard.NotNullOrWhiteSpace(RewardID, nameof(RewardID));

            if (_vesselMixer != null)
                _vesselMixer.Interacted -= OnToggleColorMixerMode;

            if (_reverseButtonView != null)
            {
                _reverseButtonView.ButtonClicked -= OnToggleMode;
                _modeTracker.ReverseButtonActivated -= _reverseButtonView.ActivateRewardedIcon;
            }

            _modeTracker.ModeChanged -= OnModeChanged;
            _reverseButtonView = reverseButtonView;
            _rewardFlow = new ReverseRewardFlow(
                _modeTracker,
                new RewardedAdvertisingGateway(),
                _reverseButtonView,
                RewardID);
            _rewardFlow.Ended += () => RewardedEnded?.Invoke();
            _reverseButtonView.ButtonClicked += OnToggleMode;
            _vesselMixer.Interacted += OnToggleColorMixerMode;
            _modeTracker.ReverseButtonActivated +=
                _reverseButtonView.ActivateRewardedIcon;
            _modeTracker.ModeChanged += OnModeChanged;

            OnModeChanged(CurrentMode);
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

        public void OnMixFailed(MagicCell firstCell, MagicCell secondCell)
        {
            ColorsCannotBeMixed?.Invoke(firstCell, secondCell);
        }

        public void OnMixerCellSent()
        {
            if (CurrentMode != ClickImpactMode.ModeColorMixing)
                return;

            _modeTracker.ActivateDistributionMode();
        }

        private void OnModeChanged(ClickImpactMode mode)
        {
            if (_vesselMixer == null)
                return;

            if (mode == ClickImpactMode.ModeColorMixing)
                _vesselMixer.Activate();
            else
                _vesselMixer.Deactivate();
        }

        private void UpdateButtonState()
        {
            Guard.NotNull(_reverseButtonView, nameof(_reverseButtonView));
            _reverseButtonView.SetReverseUsedState(_modeTracker.IsReverseUsed);
        }
    }
}