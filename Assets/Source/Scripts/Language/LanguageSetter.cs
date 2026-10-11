using Assets.Source.Scripts.Extensions;
using System;
using YG;

namespace Assets.Source.Scripts.Language
{
    public class LanguageSetter : IDisposable
    {
        private bool _disposed;

        public LanguageSetter()
        {
            //CurrentLanguage = YG2.lang;
            //YG2.onCorrectLang += OnHandleLang;

            if (YG2.saves.Language.HasSavedLanguage)
            {
                CurrentLanguage = YG2.saves.Language.SavedLanguage;

                YG2.SwitchLanguage(CurrentLanguage);
            }
            else
            {
                CurrentLanguage = YG2.lang;
            }

            YG2.onCorrectLang += OnHandleLang;
        }

        public event Action<string> OnLanguageChanged;

        public string CurrentLanguage { get; private set; }

        public void Dispose()
        {
            if (_disposed)
                return;

            YG2.onCorrectLang -= OnHandleLang;
            _disposed = true;

            GC.SuppressFinalize(this);
        }

        public void SetLanguage(string language)
        {
            Guard.NotNullOrWhiteSpace(language, nameof(language));

            if (CurrentLanguage == language)
                return;

            CurrentLanguage = language;

            YG2.saves.SaveLanguage(language);
            YG2.SwitchLanguage(language);
            OnLanguageChanged?.Invoke(language);
        }

        private void OnHandleLang(string language)
        {
            CurrentLanguage = language;

            OnLanguageChanged?.Invoke(language);
        }
    }
}