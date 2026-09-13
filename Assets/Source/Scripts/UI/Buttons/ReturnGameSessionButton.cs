namespace Assets.Source.Scripts.UI.Buttons
{
    public class ReturnGameSessionButton : BaseButton
    {
        protected override void OnButtonClick()
        {
            CurrentPanel?.Close();
            TargetPanel?.Open();
            GameHandler.ContinueGame();
        }
    }
}