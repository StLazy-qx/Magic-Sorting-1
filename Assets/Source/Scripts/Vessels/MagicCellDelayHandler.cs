using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.Tutorial;
using System.Threading;
using System;

namespace Assets.Source.Scripts.Vessels
{
    class MagicCellDelayHandler
    {
        private readonly AsyncTimer _timer;
        private readonly float _delay;
        private bool _isProcessing;
        private readonly CancellationTokenSource _lifetimeCts;

        public MagicCellDelayHandler(float delay)
        {
            Guard.Positive((int)delay, nameof(delay));

            _delay = delay;
            _timer = new AsyncTimer();
            _isProcessing = false;
            _lifetimeCts = new CancellationTokenSource();
        }

        public void TakeMagic(MagicCell cell, Action onComplete)
        {
            Guard.NotNull(cell, nameof(cell));
            Guard.NotNull(onComplete, nameof(onComplete));

            if (_isProcessing)
                return;

            _isProcessing = true;

            _timer.StartTimer(_delay, () =>
            {
                _isProcessing = false;

                onComplete?.Invoke();
            }, 
            _lifetimeCts.Token);
        }

        public void Cancel()
        {
            _timer.StopTimer();

            _isProcessing = false;
        }

        public void Dispose()
        {
            Cancel();
            _lifetimeCts.Cancel();
            _lifetimeCts.Dispose();
        }
    }
}
