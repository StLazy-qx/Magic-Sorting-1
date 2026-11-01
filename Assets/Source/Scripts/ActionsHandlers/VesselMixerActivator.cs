using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Color_Mixing_Mechanics;
using Assets.Source.Scripts.Extensions;

namespace Assets.Source.Scripts.ActionsHandlers
{
    public class VesselMixerActivator : IModeListener
    {
        private readonly VesselMixer _vesselMixer;

        public VesselMixerActivator(VesselMixer vesselMixer)
        {
            Guard.NotNull(vesselMixer, nameof(vesselMixer));

            _vesselMixer = vesselMixer;
        }

        public void OnModeChanged(ClickImpactMode mode)
        {
            if (mode == ClickImpactMode.ModeColorMixing)
                _vesselMixer.Activate();
            else
                _vesselMixer.Deactivate();
        }
    }
}
