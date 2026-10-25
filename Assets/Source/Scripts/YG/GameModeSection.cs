using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Player;
using System;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class GameModeSection : BaseSection
    {
        private const int NotSaved = -1;

        public GameModeSection(SavesYG saves) : base(saves) { }

        public bool HasSavedMode =>
            _saves.SavedGameMode != NotSaved
            && _saves.SavedDifficultyLevel != NotSaved
            && Enum.IsDefined(typeof(GameMode), _saves.SavedGameMode)
            && Enum.IsDefined(typeof(DifficultyLevel), _saves.SavedDifficultyLevel);

        public GameMode GetGameMode()
            => (GameMode)_saves.SavedGameMode;

        public DifficultyLevel GetDifficultyLevel()
            => (DifficultyLevel)_saves.SavedDifficultyLevel;

        public void SaveMode(GameMode mode, DifficultyLevel level)
        {
            _saves.SavedGameMode = (int)mode;
            _saves.SavedDifficultyLevel = (int)level;

            SaveProgress();
        }
    }
}