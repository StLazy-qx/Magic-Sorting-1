using Assets.Source.Scripts.Extensions;
using YG;

namespace Assets.Source.Scripts.Audio
{
    public class AudioSettingsData
    {
        private const float MinVolume = 0f;
        private const float MaxVolume = 1f;

        public float Master { get; private set; }
        public float Ambient { get; private set; }
        public float Effect { get; private set; }

        public void Load()
        {
            Master = ValidateVolumeValue(YG2.saves.MasterVolume, nameof(Master));
            Ambient = ValidateVolumeValue(YG2.saves.AmbientVolume, nameof(Ambient));
            Effect = ValidateVolumeValue(YG2.saves.EffectVolume, nameof(Effect));
        }

        public void UpdateMasterVolume(float value)
        {
            Master = ValidateVolumeValue(value, nameof(Master));
        }

        public void UpdateAmbientVolume(float value)
        {
            Ambient = ValidateVolumeValue(value, nameof(Ambient));
        }

        public void UpdateEffectVolume(float value)
        {
            Effect = ValidateVolumeValue(value, nameof(Effect));
        }

        public void SetMasterVolume(float value)
        {
            Master = ValidateVolumeValue(value, nameof(Master));

            YG2.saves.SaveMasterVolume(value);
        }

        public void SetAmbientVolume(float value)
        {
            Ambient = ValidateVolumeValue(value, nameof(Ambient));

            YG2.saves.SaveAmbientVolume(value);
        }

        public void SetEffectVolume(float value)
        {
            Effect = ValidateVolumeValue(value, nameof(Effect));

            YG2.saves.SaveEffectVolume(value);
        }

        private float ValidateVolumeValue(float value, string parameterName)
        {
            Guard.InRange(value, MinVolume, MaxVolume, parameterName);

            return value;
        }
    }
}