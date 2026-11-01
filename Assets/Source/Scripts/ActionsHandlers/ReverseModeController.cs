using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.UI.GameModeView;
using Assets.Source.Scripts.YG;
using System;

namespace Assets.Source.Scripts.ActionsHandlers
{
    public class ReverseModeController
    {
        //private readonly ClickModeTracker _modeTracker;
        //private readonly RewardedAdvertisingGateway _rewardedGateway;

        //private ReverseButtonView _view;
        //private ReverseRewardFlow _rewardFlow;

        //public event Action RewardedEnded;

        //public ReverseModeController(
        //    ClickModeTracker modeTracker,
        //    RewardedAdvertisingGateway rewardedGateway)
        //{
        //    Guard.NotNull(modeTracker, nameof(modeTracker));
        //    Guard.NotNull(rewardedGateway, nameof(rewardedGateway));

        //    _modeTracker = modeTracker;
        //    _rewardedGateway = rewardedGateway;
        //}

        //public bool IsBound => _view != null;

        //public void Bind(ReverseButtonView view, string rewardId)
        //{
        //    Guard.NotNull(view, nameof(view));
        //    Guard.NotNullOrWhiteSpace(rewardId, nameof(rewardId));
        //    Unbind();

        //    _view = view;
        //    _rewardFlow = new ReverseRewardFlow(
        //        _modeTracker, _rewardedGateway, view, rewardId);
        //    _rewardFlow.Ended += RaiseRewardedEnded;
        //    _view.ButtonClicked += Toggle;
        //    _modeTracker.ReverseButtonActivated += _view.ActivateRewardedIcon;

        //    UpdateButtonState();
        //}

        //public void BindForTutorial(ReverseButtonView view)
        //{
        //    Guard.NotNull(view, nameof(view));

        //    Unbind();

        //    _view = view;

        //    _view.HideRewardedIconPermanently();

        //    _view.ButtonClicked += ToggleForTutorial;
        //}

        //public void Unbind()
        //{
        //    if (_view == null) return;

        //    _view.ButtonClicked -= Toggle;
        //    _view.ButtonClicked -= ToggleForTutorial;
        //    _modeTracker.ReverseButtonActivated -= _view.ActivateRewardedIcon;

        //    if (_rewardFlow != null)
        //        _rewardFlow.Ended -= RaiseRewardedEnded;

        //    _rewardFlow = null;
        //    _view = null;
        //}

        //public void Toggle()
        //{
        //    Guard.NotNull(_view, nameof(_view));

        //    if (CurrentModeIsColorMixingOrReverse())
        //    {
        //        _modeTracker.ActivateDistributionMode();

        //        return;
        //    }

        //    if (_modeTracker.IsReverseUsed == false)
        //    {
        //        _modeTracker.ActivateReverseMode();

        //        return;
        //    }

        //    if (_view.IsRewardedIconActive == false)
        //    {
        //        _modeTracker.ActivateReverseMode();

        //        return;
        //    }

        //    _rewardFlow.Show();
        //}

        //public void ToggleForTutorial()
        //{
        //    Guard.NotNull(_view, nameof(_view));

        //    _modeTracker.Activate(
        //        _modeTracker.CurrentMode == ClickImpactMode.ModeReverse
        //            ? ClickImpactMode.ModeDistribution
        //            : ClickImpactMode.ModeReverse);
        //}

        //public void MarkReverseUsed()
        //{
        //    if (_modeTracker.CurrentMode != ClickImpactMode.ModeReverse) return;

        //    _modeTracker.MarkReverseUsed();
        //    UpdateButtonState();
        //}

        //public void Reset()
        //{
        //    _view?.ResetState();
        //    UpdateButtonState();
        //}

        //private bool CurrentModeIsColorMixingOrReverse() =>
        //    _modeTracker.CurrentMode == ClickImpactMode.ModeColorMixing ||
        //    _modeTracker.CurrentMode == ClickImpactMode.ModeReverse;

        //private void UpdateButtonState() =>
        //    _view?.SetReverseUsedState(_modeTracker.IsReverseUsed);

        //private void RaiseRewardedEnded() 
        //    => RewardedEnded?.Invoke();
    }
}