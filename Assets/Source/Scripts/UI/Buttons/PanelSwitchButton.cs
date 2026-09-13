namespace Assets.Source.Scripts.UI.Buttons
{
    public class PanelSwitchButton : BaseButton
    {
        protected override void OnButtonClick()
        {
            CurrentPanel?.Close();
            TargetPanel?.Open();
        }
    }
}