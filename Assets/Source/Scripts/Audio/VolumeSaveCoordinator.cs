using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using YG;

namespace Assets.Source.Scripts.Audio
{
    public class VolumeSaveCoordinator
    {
        private const float SaveThreshold = 0.1f;
        private const int CheckIntervalMilliseconds = 500;

        private readonly AudioSettingsData _settings;
        private readonly SoundSetter _soundSetter;

        private CancellationTokenSource _cancellationTokenSource;

        private float _masterIntervalStart;
        private float _ambientIntervalStart;
        private float _effectIntervalStart;

        public VolumeSaveCoordinator(AudioSettingsData settings, SoundSetter soundSetter)
        {
            _settings = settings;
            _soundSetter = soundSetter;
        }

        public void Initialize()
        {
            _masterIntervalStart = _settings.Master;
            _ambientIntervalStart = _settings.Ambient;
            _effectIntervalStart = _settings.Effect;

            // Подписка на события SoundSetter
            _soundSetter.MasterVolumeChanged += OnMasterVolumeChanged;
            _soundSetter.AmbientVolumeChanged += OnAmbientVolumeChanged;
            _soundSetter.EffectVolumeChanged += OnEffectVolumeChanged;

            _soundSetter.MasterReleased += SaveMasterOnRelease;
            _soundSetter.AmbientReleased += SaveAmbientOnRelease;
            _soundSetter.EffectReleased += SaveEffectOnRelease;

            // Запуск цикла проверки
            _cancellationTokenSource = new CancellationTokenSource();
            RunComparisonLoop(_cancellationTokenSource.Token).Forget();
        }

        private void OnMasterVolumeChanged(float value)
        {
            _settings.UpdateMasterVolume(value);
        }

        private void OnAmbientVolumeChanged(float value)
        {
            _settings.UpdateAmbientVolume(value);
        }

        private void OnEffectVolumeChanged(float value)
        {
            _settings.UpdateEffectVolume(value);
        }

        private async UniTaskVoid RunComparisonLoop(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    await UniTask.Delay(
                        CheckIntervalMilliseconds,
                        cancellationToken: cancellationToken);

                    CompareAndSaveVolumes();
                }
            }
            catch (OperationCanceledException) { }
        }

        private void CompareAndSaveVolumes()
        {
            CheckParameter(
                _settings.Master,
                _masterIntervalStart,
                value => _settings.SetMasterVolume(value),
                newValue => _masterIntervalStart = newValue);

            CheckParameter(
                _settings.Ambient,
                _ambientIntervalStart,
                value => _settings.SetAmbientVolume(value),
                newValue => _ambientIntervalStart = newValue);

            CheckParameter(
                _settings.Effect,
                _effectIntervalStart,
                value => _settings.SetEffectVolume(value),
                newValue => _effectIntervalStart = newValue);
        }

        private void CheckParameter(
            float currentValue,
            float intervalStart,
            Action<float> saveAction,
            Action<float> updateStartAction)
        {
            if (Mathf.Abs(currentValue - intervalStart) > SaveThreshold)
            {
                saveAction(currentValue);
            }
            updateStartAction(currentValue);
        }

        private void SaveMasterOnRelease()
        {
            _settings.SetMasterVolume(_settings.Master);
            YG2.saves.ForceSaveAudio();
            _masterIntervalStart = _settings.Master;
        }

        private void SaveAmbientOnRelease()
        {
            _settings.SetAmbientVolume(_settings.Ambient);
            YG2.saves.ForceSaveAudio();
            _ambientIntervalStart = _settings.Ambient;
        }

        private void SaveEffectOnRelease()
        {
            _settings.SetEffectVolume(_settings.Effect);
            YG2.saves.ForceSaveAudio();
            _effectIntervalStart = _settings.Effect;
        }

        public void SaveAllVolumes()
        {
            _settings.SetMasterVolume(_settings.Master);
            _settings.SetAmbientVolume(_settings.Ambient);
            _settings.SetEffectVolume(_settings.Effect);
            YG2.saves.ForceSaveAudio();
        }

        public void Dispose()
        {
            _soundSetter.MasterVolumeChanged -= OnMasterVolumeChanged;
            _soundSetter.AmbientVolumeChanged -= OnAmbientVolumeChanged;
            _soundSetter.EffectVolumeChanged -= OnEffectVolumeChanged;

            _soundSetter.MasterReleased -= SaveMasterOnRelease;
            _soundSetter.AmbientReleased -= SaveAmbientOnRelease;
            _soundSetter.EffectReleased -= SaveEffectOnRelease;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
