using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.Tutorial;
using System.Threading;
using System.Collections.Generic;
using System;

namespace Assets.Source.Scripts.Vessels
{
    class MagicCellDelayHandler
    {
        private readonly float _delay;
        private readonly CancellationTokenSource _lifetimeCts;
        private readonly List<AsyncTimer> _timers = new();

        public MagicCellDelayHandler(float delay)
        {
            Guard.Positive((int)delay, nameof(delay));

            _delay = delay;
            _lifetimeCts = new CancellationTokenSource();
        }

        public void TakeMagic(MagicCell cell, Action onComplete)
        {
            Guard.NotNull(cell, nameof(cell));
            Guard.NotNull(onComplete, nameof(onComplete));

            var timer = new AsyncTimer();

            _timers.Add(timer);

            timer.StartTimer(
                _delay,
                () =>
                {
                    _timers.Remove(timer);
                    onComplete.Invoke();
                },
                _lifetimeCts.Token);
        }

        public void Cancel()
        {
            foreach (var timer in _timers)
                timer.StopTimer();

            _timers.Clear();
        }

        public void Dispose()
        {
            Cancel();
            _lifetimeCts.Cancel();
            _lifetimeCts.Dispose();
        }
    }
}