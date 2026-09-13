using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class AudioSaveScheduler : IDisposable
    {
        private const int SaveIntervalMilliseconds = 500;

        private CancellationTokenSource _cancellationTokenSource;
        private bool _saveRequested;
        private bool _isSaving;

        public AudioSaveScheduler()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            RunSaveLoop(_cancellationTokenSource.Token).Forget();
        }

        public void RequestSave()
        {
            _saveRequested = true;
        }

        public void ForceSaver()
        {
            if (_saveRequested == false)
                return;

            YG2.SaveProgress();

            _saveRequested = false;
        }

        private async UniTaskVoid RunSaveLoop(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    await UniTask.Delay(
                        SaveIntervalMilliseconds,
                        cancellationToken: cancellationToken);

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
            catch (OperationCanceledException) {}
        }

        public void Dispose()
        {
            ForceSaver();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            _cancellationTokenSource = null;
        }
    }
}
