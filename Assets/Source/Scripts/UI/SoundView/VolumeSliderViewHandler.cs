using Assets.Source.Scripts.Extensions;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace Assets.Source.Scripts.UI.SoundView
{
    public class VolumeSliderViewHandler : MonoBehaviour
    {
        [SerializeField] private Slider _sliderMasterVolume;
        [SerializeField] private Slider _sliderAmbientVolume;
        [SerializeField] private Slider _sliderEffectVolume;
        [SerializeField] private SliderHandleReleaseListener _masterHandleRelease;
        [SerializeField] private SliderHandleReleaseListener _ambientHandleRelease;
        [SerializeField] private SliderHandleReleaseListener _effectHandleRelease;

        public event Action<float> OnMasterChanged;
        public event Action<float> OnAmbientChanged;
        public event Action<float> OnEffectChanged;
        public event Action OnMasterReleased;
        public event Action OnAmbientReleased;
        public event Action OnEffectReleased;

        private void Awake()
        {
            ValidateInitializeArguments();

            _sliderMasterVolume.onValueChanged.AddListener(
                value => OnMasterChanged?.Invoke(value));
            _sliderAmbientVolume.onValueChanged.AddListener(
                value => OnAmbientChanged?.Invoke(value));
            _sliderEffectVolume.onValueChanged.AddListener(
                value => OnEffectChanged?.Invoke(value));

            _masterHandleRelease.Released += () => OnMasterReleased?.Invoke();
            _ambientHandleRelease.Released += () => OnAmbientReleased?.Invoke();
            _effectHandleRelease.Released += () => OnEffectReleased?.Invoke();
        }

        public void SetInitialValues(float master, float ambient, float effect)
        {
            ValidateVolumeValues(master, ambient, effect);
            _sliderMasterVolume.SetValueWithoutNotify(master);
            _sliderAmbientVolume.SetValueWithoutNotify(ambient);
            _sliderEffectVolume.SetValueWithoutNotify(effect);
        }

        private void ValidateInitializeArguments()
        {
            Guard.NotNull(_sliderMasterVolume, nameof(_sliderMasterVolume));
            Guard.NotNull(_sliderAmbientVolume, nameof(_sliderAmbientVolume));
            Guard.NotNull(_sliderEffectVolume, nameof(_sliderEffectVolume));
            Guard.NotNull(_masterHandleRelease, nameof(_masterHandleRelease));
            Guard.NotNull(_ambientHandleRelease, nameof(_ambientHandleRelease));
            Guard.NotNull(_effectHandleRelease, nameof(_effectHandleRelease));
        }

        private void ValidateVolumeValues(float master, float ambient, float effect)
        {
            float maxVolume = 1f;
            float minVolume = 0f;

            Guard.InRange(master, minVolume, maxVolume, nameof(master));
            Guard.InRange(ambient, minVolume, maxVolume, nameof(ambient));
            Guard.InRange(effect, minVolume, maxVolume, nameof(effect));
        }
    }
}