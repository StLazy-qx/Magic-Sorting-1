namespace Assets.Source.Scripts.GameDifficulty
{
    public class DifficultySettingsProvider
    {
        private readonly DifficultyDatabase _database;
        private readonly DifficultyState _state;

        public DifficultySettingsProvider(
            DifficultyDatabase database,
            DifficultyState state)
        {
            _database = database;
            _state = state;
        }

        public DifficultySettings Current =>
            _database.GetSettings(_state.CurrentDifficulty);
    }
}
