using Assets.Source.Scripts.Save_Policies;
using System;
using System.Collections.Generic;
using YG;

namespace Assets.Source.Scripts.YG
{
    public abstract class BaseSection
    {
        protected readonly SavesYG _savesContainer;
        protected readonly ISavePolicy _savePolicy;

        protected BaseSection(SavesYG savesContainer, ISavePolicy savePolicy)
        {
            _savesContainer = savesContainer;
            _savePolicy = savePolicy;
        }

        public virtual void Rebuild() 
        { }

        public virtual void Flush() 
            => _savePolicy.Flush();

        protected void Mutate<T>(
            ref T targetField, 
            T newValue, 
            Action<T> validateAction = null)
        {
            validateAction?.Invoke(newValue);

            if (EqualityComparer<T>.Default.Equals(targetField, newValue)) 
                return;

            targetField = newValue;

            _savePolicy.RequestSave();
        }

        //protected readonly SavesYG _saves;

        //protected BaseSection(SavesYG saves)
        //{
        //    _saves = saves;
        //}

        //protected void SaveProgress()
        //{
        //    YG2.SaveProgress();
        //}
    }
}
