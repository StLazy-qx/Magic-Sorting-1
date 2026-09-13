using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using System;

namespace Assets.Source.Scripts.ActionsHandlers
{
    public class ClickModeTracker
    {
        public ClickImpactMode CurrentMode { get; private set; }
        public bool IsReverseUsed { get; private set; }
        public bool ReverseEventFired { get; private set; }

        public event Action<ClickImpactMode> ModeChanged;
        public event Action ReverseButtonActivated;

        public void ActivateDistributionMode()
        {
            CurrentMode = ClickImpactMode.ModeDistribution;

            ModeChanged?.Invoke(CurrentMode);
        }

        public void ActivateReverseMode()
        {
            CurrentMode = ClickImpactMode.ModeReverse;

            ModeChanged?.Invoke(CurrentMode);

            if (ReverseEventFired == false)
            {
                ReverseEventFired = true;

                ReverseButtonActivated?.Invoke();
            }
        }

        public void MarkReverseUsed()
        {
            Guard.IsTrue(CurrentMode == ClickImpactMode.ModeReverse,
                nameof(CurrentMode), 
                "Cannot mark reverse used when not in Reverse mode");

            IsReverseUsed = true;
        }

        public void Reset()
        {
            IsReverseUsed = false;
            ReverseEventFired = false;

            ActivateDistributionMode();
        }
    }
}
