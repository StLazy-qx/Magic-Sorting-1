using Assets.Source.Scripts.GameBehaviour;

namespace Assets.Source.Scripts.UI.Buttons
{
    public class ButtonBeginRound : BaseButton
    {
        protected override void OnButtonClick()
        {
            if (GameHandler is IGameHandler gameHandler)
            {
                CurrentPanel?.Close();
                TargetPanel?.Open();
                gameHandler.BeginRound();
            }
        }
    }
}