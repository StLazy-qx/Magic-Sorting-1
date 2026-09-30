using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;

namespace Assets.Source.Scripts.GameDifficulty
{
    public class DifficultySettingsProvider
    {
        private readonly DifficultyDatabase _database;
        private readonly DifficultyState _difficultyState;

        public DifficultySettingsProvider(
            DifficultyDatabase database,
            DifficultyState difficultyState)
        {
            Guard.NotNull(database, nameof(database));
            Guard.NotNull(difficultyState, nameof(difficultyState));

            _database = database;
            _difficultyState = difficultyState;
        }

        public DifficultyLevel CurrentDifficulty =>
            _difficultyState.CurrentDifficulty;

        public DifficultySettings CurrentSettings =>
            GetSetting(_difficultyState.CurrentDifficulty);

        public DifficultySettings GetSetting(DifficultyLevel level) =>
            _database.GetSettings(level);

        public void SetDifficulty(DifficultyLevel level) =>
            _difficultyState.SetDifficulty(level);

        public DifficultyLevel GetIncreasedDifficulty(DifficultyLevel current)
        {
            switch (current)
            {
                case DifficultyLevel.Easy:
                    return DifficultyLevel.Medium;

                case DifficultyLevel.Medium:
                    return DifficultyLevel.Hard;

                case DifficultyLevel.Hard:
                    return DifficultyLevel.Hard;

                default:
                    return current;
            }
        }
    }
}
