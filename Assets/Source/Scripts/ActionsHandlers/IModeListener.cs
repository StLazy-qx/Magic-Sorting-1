using Assets.Source.Scripts.Enums;

namespace Assets.Source.Scripts.ActionsHandlers
{
    public interface IModeListener
    {
        public void OnModeChanged(ClickImpactMode mode);
    }
}