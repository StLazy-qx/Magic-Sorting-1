using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Save_Policies;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class LanguageSection : BaseSection
    {
        public LanguageSection(SavesYG savesContainer, ISavePolicy savePolicy)
            : base(savesContainer, savePolicy) { }

        public string Saved => _savesContainer.SavedLanguage;

        public bool HasSavedLanguage
            => !string.IsNullOrEmpty(_savesContainer.SavedLanguage);

        public void Save(string language)
            => Mutate(
                ref _savesContainer.SavedLanguage,
                language,
                validatedLanguage => Guard.NotNullOrWhiteSpace(validatedLanguage, nameof(language)));
    }

    //public class LanguageSection : BaseSection
    //{
    //    public LanguageSection(SavesYG saves) : base(saves) { }

    //    public string SavedLanguage => _saves.SavedLanguage;

    //    public bool HasSavedLanguage => string.IsNullOrEmpty(_saves.SavedLanguage) == false;

    //    public void SaveLanguage(string language)
    //    {
    //        Guard.NotNullOrWhiteSpace(language, nameof(language));

    //        if (_saves.SavedLanguage == language)
    //            return;

    //        _saves.SavedLanguage = language;

    //        SaveProgress();
    //    }
    //}
}
