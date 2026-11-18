using YG;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class GameSessionHandler : BaseGameHandler
    {
        private bool _isAdShowing;

        private void OnEnable()
        {
            YG2.onCloseInterAdv += HandleAdClosed;
            YG2.onCloseInterAdvWasShow += OnCloseInterAdvWasShow;
            YG2.onErrorInterAdv += HandleAdClosed;
        }

        private void OnDisable()
        {
            YG2.onCloseInterAdv -= HandleAdClosed;
            YG2.onCloseInterAdvWasShow -= OnCloseInterAdvWasShow;
            YG2.onErrorInterAdv -= HandleAdClosed;
        }

        public void ShowInterstitialAdvertising()
        {
            if (_isAdShowing)
                return;

            _isAdShowing = true;

            YG2.InterstitialAdvShow();
        }

        private void OnCloseInterAdvWasShow(bool wasShown)
            => HandleAdClosed();

        private void HandleAdClosed()
            => _isAdShowing = false;
    }
}