using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.UI.SoundView;
using System;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Assets.Source.Scripts.Audio
{
    public class SoundSetter : MonoBehaviour, IObjectInitilizable
    {
        private const string Master = "Master";
        private const string Ambient = "Ambient";
        private const string Effect = "Effect";
        private const float MinVolume = 0.0001f;
        private const float MaxVolume = 1f;
        private const float VolumeMultiplier = 20f;

        [SerializeField] private AudioMixer _mixer;

        private AudioSettingsData _settings;
        private VolumeSliderViewHandler _volumeSliderView;
        private VolumeSaveCoordinator _saveCoordinator;

        public event Action<float> MasterVolumeChanged;
        public event Action<float> AmbientVolumeChanged;
        public event Action<float> EffectVolumeChanged;
        public event Action MasterReleased;
        public event Action AmbientReleased;
        public event Action EffectReleased;

        public bool IsInitialized { get; private set; }

        [Inject]
        public void Construct(AudioSettingsData settings)
        {
            Guard.NotNull(settings, nameof(settings));

            _settings = settings;
        }

        public void Initialize()
        {
            ValidateDependencies();
            _settings.Load();

            _volumeSliderView.OnMasterChanged += HandleMasterChanged;
            _volumeSliderView.OnAmbientChanged += HandleAmbientChanged;
            _volumeSliderView.OnEffectChanged += HandleEffectChanged;
            _volumeSliderView.OnMasterReleased += () => MasterReleased?.Invoke();
            _volumeSliderView.OnAmbientReleased += () => AmbientReleased?.Invoke();
            _volumeSliderView.OnEffectReleased += () => EffectReleased?.Invoke();

            ValidateSettingsValues();
            _volumeSliderView.SetInitialValues(
                _settings.Master,
                _settings.Ambient,
                _settings.Effect
            );

            RestoreVolumes();

            _saveCoordinator = new VolumeSaveCoordinator(_settings, this);

            _saveCoordinator.Initialize();

            IsInitialized = true;
        }

        public void ApplyAudioHandler(VolumeSliderViewHandler volumeSliderView)
        {
            Guard.NotNull(volumeSliderView, nameof(volumeSliderView));

            _volumeSliderView = volumeSliderView;
        }

        private void HandleMasterChanged(float value)
        {
            SetVolume(Master, value);
            _settings.UpdateMasterVolume(value);
            MasterVolumeChanged?.Invoke(value);
        }

        private void HandleAmbientChanged(float value)
        {
            SetVolume(Ambient, value);
            _settings.UpdateAmbientVolume(value);
            AmbientVolumeChanged?.Invoke(value);
        }

        private void HandleEffectChanged(float value)
        {
            SetVolume(Effect, value);
            _settings.UpdateEffectVolume(value);
            EffectVolumeChanged?.Invoke(value);
        }

        private void SetVolume(string parameter, float value)
        {
            float db = Mathf.Log10(Mathf.Clamp(
                value, MinVolume, MaxVolume)) * VolumeMultiplier;

            _mixer.SetFloat(parameter, db);
        }
        
        private void RestoreVolumes()
        {
            SetVolume(Master, _settings.Master);
            SetVolume(Ambient, _settings.Ambient);
            SetVolume(Effect, _settings.Effect);
        }

        private void ValidateSettingsValues()
        {
            ValidateVolumeValue(_settings.Master, Master);
            ValidateVolumeValue(_settings.Ambient, Ambient);
            ValidateVolumeValue(_settings.Effect, Effect);
        }

        private void ValidateDependencies()
        {
            Guard.NotNull(_mixer, nameof(_mixer));
            Guard.NotNull(_settings, nameof(_settings));
            Guard.NotNull(_volumeSliderView, nameof(_volumeSliderView));
        }

        private void ValidateVolumeValue(float volume, string name)
        {
            Guard.InRange(volume, 0f, MaxVolume, name);
        }
    }
}