using Assets.Source.Scripts.GameBehaviour;

namespace Assets.Source.Scripts.UI.Buttons
{
    public class ButtonAdvertisingActivater : BaseButton
    {
        protected override void OnButtonClick()
        {
            if (GameHandler is not GameSessionHandler sessionHandler)
                return;

            sessionHandler.ShowInterstitialAd();
        }
    }
}