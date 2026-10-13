using System;
using YG;


namespace Assets.Source.Scripts.YG
{
    public class YG2InterstitialBridge
    {
        private bool _isShowing;

        public event Action Closed;

        public void Dispose() => Disable();

        public void Enable()
        {
            YG2.onCloseInterAdv += HandleClosed;
            YG2.onCloseInterAdvWasShow += HandleClosedShow;
            YG2.onErrorInterAdv += HandleError;
        }

        public void Disable()
        {
            YG2.onCloseInterAdv -= HandleClosed;
            YG2.onCloseInterAdvWasShow -= HandleClosedShow;
            YG2.onErrorInterAdv -= HandleError;
        }

        public void Show()
        {
            if (_isShowing)
                return;

            _isShowing = true;

            YG2.InterstitialAdvShow();
        }

        private void HandleClosed()
            => Finish();

        private void HandleClosedShow(bool _)
            => Finish();

        private void HandleError()
            => Finish();

        private void Finish()
        {
            _isShowing = false;

            Closed?.Invoke();
        }
    }
}
