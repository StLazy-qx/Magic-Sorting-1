using UnityEngine;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class AudioSettingsSection
    {
        private readonly SavesYG _saves;
        private readonly AudioSaveScheduler _saveScheduler;

        public AudioSettingsSection(SavesYG saves)
        {
            _saves = saves;
            _saveScheduler = new AudioSaveScheduler();
        }

        public float MasterVolume => _saves.MasterVolume;
        public float AmbientVolume => _saves.AmbientVolume;
        public float EffectVolume => _saves.EffectVolume;

        public void SaveMasterVolume(float value)
        {
            float clamped = Mathf.Clamp01(value);

            if (_saves.MasterVolume == clamped) 
                return;

            _saves.MasterVolume = clamped;

            _saveScheduler.RequestSave();
        }

        public void SaveAmbientVolume(float value)
        {
            float clamped = Mathf.Clamp01(value);

            if (_saves.AmbientVolume == clamped) 
                return;

            _saves.AmbientVolume = clamped;

            _saveScheduler.RequestSave();
        }

        public void SaveEffectVolume(float value)
        {
            float clamped = Mathf.Clamp01(value);

            if (_saves.EffectVolume == clamped) 
                return;

            _saves.EffectVolume = clamped;

            _saveScheduler.RequestSave();
        }

        public void ForceSaver()
        {
            _saveScheduler.ForceSaver();
        }

        public void Dispose()
        {
            _saveScheduler.Dispose();
        }
    }
}
