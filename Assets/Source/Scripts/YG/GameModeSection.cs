using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.Save_Policies;
using System;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class GameModeSection : BaseSection
    {
        private const int NotSaved = 0;

        public GameModeSection(SavesYG savesContainer, ISavePolicy savePolicy)
            : base(savesContainer, savePolicy) { }

        public bool HasSavedMode =>
            _savesContainer.SavedGameMode != NotSaved
            && Enum.IsDefined(typeof(GameMode), _savesContainer.SavedGameMode);

        public GameMode Load()
            => (GameMode)_savesContainer.SavedGameMode;

        public void Save(GameMode gameMode)
            => Mutate(ref _savesContainer.SavedGameMode, (int)gameMode);
    }

    //public class GameModeSection : BaseSection
    //{
    //    private const int NotSaved = 0;

    //    public GameModeSection(SavesYG saves) : base(saves) { }

    //    public bool HasSavedMode =>
    //        _saves.SavedGameMode != NotSaved
    //        && Enum.IsDefined(typeof(GameMode), _saves.SavedGameMode);

    //    public GameMode LoadMode()
    //        => (GameMode)_saves.SavedGameMode;

    //    public void SaveMode(GameMode mode)
    //    {
    //        if (_saves.SavedGameMode == (int)mode)
    //            return;

    //        _saves.SavedGameMode = (int)mode;

    //        SaveProgress();
    //    }
    //}
}