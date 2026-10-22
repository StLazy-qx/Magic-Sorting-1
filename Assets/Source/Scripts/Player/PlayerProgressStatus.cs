namespace Assets.Source.Scripts.Player
{
    public class PlayerProgressStatus
    {
        public GameMode Mode { get; }
        public int RoundNumber { get; }

        public PlayerProgressStatus(GameMode mode, int roundNumber)
        {
            Mode = mode;
            RoundNumber = roundNumber;
        }
    }
}