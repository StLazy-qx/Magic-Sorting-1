using Assets.Source.Scripts.Extensions;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class LanguageSection : BaseSection
    {
        public LanguageSection(SavesYG saves) : base(saves) { }

        public string SavedLanguage => _saves.SavedLanguage;

        public bool HasSavedLanguage => string.IsNullOrEmpty(_saves.SavedLanguage) == false;

        public void SaveLanguage(string language)
        {
            Guard.NotNullOrWhiteSpace(language, nameof(language));

            if (_saves.SavedLanguage == language)
                return;

            _saves.SavedLanguage = language;

            SaveProgress();
        }
    }
}
