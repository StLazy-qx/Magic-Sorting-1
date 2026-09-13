using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Assets.Source.Scripts.Tutorial
{
    class AsyncTimer
    {
        private CancellationTokenSource _cancellationTokenSource;

        public void StartTimer(
            float delayInSeconds, 
            Action onElapsed, 
            CancellationToken externalToken = default)
        {
            StopTimer();

            _cancellationTokenSource = 
                CancellationTokenSource.CreateLinkedTokenSource(externalToken);

            RunTimerAsync(delayInSeconds, onElapsed, _cancellationTokenSource.Token).Forget();
        }

        public void StopTimer()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private async UniTaskVoid RunTimerAsync(
            float delay, 
            Action callback, 
            CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay), 
                    cancellationToken: cancellationToken);

                callback?.Invoke();
            }
            catch (OperationCanceledException) 
            {
            }
            finally
            {
                if (_cancellationTokenSource != null && 
                    _cancellationTokenSource.Token == cancellationToken)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
            }
        }
    }
}
