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
}