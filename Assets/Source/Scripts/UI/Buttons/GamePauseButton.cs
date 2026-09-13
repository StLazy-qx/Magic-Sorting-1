namespace Assets.Source.Scripts.UI.Buttons
{
    public class GamePauseButton : BaseButton
    {
        protected override void OnButtonClick()
        {
            GameHandler.PauseGame();
            CurrentPanel?.Close();
            TargetPanel?.Open();
        }
    }
}