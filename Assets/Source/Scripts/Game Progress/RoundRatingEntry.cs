using System;

namespace Assets.Source.Scripts.Enums
{
    [Serializable]
    public struct RoundRatingEntry
    {
        public int GameMode;
        public int RoundIndex;
        public int Rating;

        public RoundRatingEntry(int gameMode, int roundIndex, int rating)
        {
            GameMode   = gameMode;
            RoundIndex = roundIndex;
            Rating     = rating;
        }
    }
}