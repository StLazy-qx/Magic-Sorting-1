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

        //public float MasterVolume => _saves.MasterVolume;
        //public float AmbientVolume => _saves.AmbientVolume;
        //public float EffectVolume => _saves.EffectVolume;

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

        //public void Dispose() 
        //    => _saveScheduler.Dispose();




        //private readonly SavesYG _saves;
        //private readonly AudioSaveScheduler _saveScheduler;

        //public AudioSettingsSection(SavesYG saves)
        //{
        //    _saves = saves;
        //    _saveScheduler = new AudioSaveScheduler();
        //}

        //public float MasterVolume => _saves.MasterVolume;
        //public float AmbientVolume => _saves.AmbientVolume;
        //public float EffectVolume => _saves.EffectVolume;

        //public void SaveMasterVolume(float value)
        //{
        //    float clamped = Mathf.Clamp01(value);

        //    if (_saves.MasterVolume == clamped) 
        //        return;

        //    _saves.MasterVolume = clamped;

        //    _saveScheduler.RequestSave();
        //}

        //public void SaveAmbientVolume(float value)
        //{
        //    float clamped = Mathf.Clamp01(value);

        //    if (_saves.AmbientVolume == clamped) 
        //        return;

        //    _saves.AmbientVolume = clamped;

        //    _saveScheduler.RequestSave();
        //}

        //public void SaveEffectVolume(float value)
        //{
        //    float clamped = Mathf.Clamp01(value);

        //    if (_saves.EffectVolume == clamped) 
        //        return;

        //    _saves.EffectVolume = clamped;

        //    _saveScheduler.RequestSave();
        //}

        //public void ForceSaver()
        //{
        //    _saveScheduler.ForceSaver();
        //}

        //public void Dispose()
        //{
        //    _saveScheduler.Dispose();
        //}
    }
}
