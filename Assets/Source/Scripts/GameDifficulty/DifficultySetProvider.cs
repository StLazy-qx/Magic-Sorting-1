namespace Assets.Source.Scripts.GameDifficulty
{
    public class DifficultySetProvider
    {
        private readonly DifficultyDatabase _database;
        private readonly DifficultyState _state;

        public DifficultySetProvider(
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
