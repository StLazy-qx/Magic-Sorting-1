namespace Assets.Source.Scripts.UI.Buttons
{
    public class ButtonTableColors : BaseButton
    {
        protected override void OnButtonClick()
        {
            TargetPanel?.Open();
        }
    }
}
