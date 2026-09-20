using UnityEngine;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class AudioSettingsSection : BaseSection
    {
        private readonly AudioSaveScheduler _saveScheduler;

        public AudioSettingsSection(SavesYG saves) : base(saves)
        {
            _saveScheduler = new AudioSaveScheduler();
        }

        public void SaveMasterVolume(float value) 
            => SaveVolume(value, v => _saves.MasterVolume = v);

        public void SaveAmbientVolume(float value) 
            => SaveVolume(value, v => _saves.AmbientVolume = v);

        public void SaveEffectVolume(float value) 
            => SaveVolume(value, v => _saves.EffectVolume = v);

        private void SaveVolume(float value, System.Action<float> setter)
        {
            float clamped = Mathf.Clamp01(value);

            setter(clamped);
            _saveScheduler.RequestSave();
        }

        public void ForceSaver() 
            => _saveScheduler.ForceSaver();
    }
}
