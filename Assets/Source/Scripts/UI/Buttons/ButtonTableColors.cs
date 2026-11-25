namespace Assets.Source.Scripts.UI.Buttons
{
    public class ButtonTableColors : BaseButton
    {
        private bool _isTableOpen;

        protected override void OnButtonClick()
        {
            if (TargetPanel == null)
            {
                return;
            }

            if (_isTableOpen)
            {
                TargetPanel.Close();
            }
            else
            {
                TargetPanel.Open();
            }

            _isTableOpen = !_isTableOpen;
        }
    }
}
