using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using YG;

namespace Assets.Source.Scripts.Save_Policies
{
    public sealed class DebouncedSavePolicy : ISavePolicy, IDisposable
    {
        private const int DefaultIntervalMilliseconds = 500;

        private readonly int _intervalMilliseconds;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _saveRequested;
        private bool _isSaving;

        public DebouncedSavePolicy(int intervalMilliseconds = DefaultIntervalMilliseconds)
        {
            _intervalMilliseconds = intervalMilliseconds;
            _cancellationTokenSource = new CancellationTokenSource();

            RunSaveLoop(_cancellationTokenSource.Token).Forget();
        }

        public void RequestSave() 
            => _saveRequested = true;

        public void Flush()
        {
            if (_saveRequested == false) 
                return;

            _saveRequested = false;

            YG2.SaveProgress();
        }

        private async UniTaskVoid RunSaveLoop(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    await UniTask.Delay(_intervalMilliseconds, cancellationToken: cancellationToken);

                    if (_saveRequested == false || _isSaving) 
                        continue;

                    _isSaving = true;

                    try
                    {
                        YG2.SaveProgress();

                        _saveRequested = false;
                    }
                    finally 
                    { 
                        _isSaving = false; 
                    }
                }
            }
            catch (OperationCanceledException) { }
        }

        public void Dispose()
        {
            Flush();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            _cancellationTokenSource = null;
        }
    }
}
