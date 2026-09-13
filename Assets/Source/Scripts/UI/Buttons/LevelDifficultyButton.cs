using Assets.Source.Scripts.GameBehaviour;

namespace Assets.Source.Scripts.UI.Buttons
{
    public class LevelDifficultyButton : BaseButton
    {
        protected override void OnButtonClick()
        {
            if (GameHandler is not GameSessionHandler sessionHandler)
                return;

            CurrentPanel?.Close();
            TargetPanel?.Open();
        }
    }
}