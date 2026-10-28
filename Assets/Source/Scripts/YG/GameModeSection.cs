using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Player;
using System;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class GameModeSection : BaseSection
    {
        private const int NotSaved = 0;

        public GameModeSection(SavesYG saves) : base(saves) { }

        public bool HasSavedMode =>
            _saves.SavedGameMode != NotSaved
            && Enum.IsDefined(typeof(GameMode), _saves.SavedGameMode);

        public GameMode LoadMode()
            => (GameMode)_saves.SavedGameMode;

        public void SaveMode(GameMode mode)
        {
            if (_saves.SavedGameMode == (int)mode)
                return;

            _saves.SavedGameMode = (int)mode;

            SaveProgress();
        }
    }
}