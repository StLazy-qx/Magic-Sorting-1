#if UNITY_EDITOR
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Game_Modes;
using Assets.Source.Scripts.Player;
using UnityEngine;
using YG;

namespace Assets.Source.Scripts.DebugTools
{
    public static class DebugSavesSeeder
    {
        public static void SeedRatingsForAllRounds(IGameModeDataProvider modes)
        {
            YG2.saves.Round.DebugSeedRoundNumber(1);
            SeedModeRatings(GameMode.Easy, modes.Get(GameMode.Easy).RoundsCount);
            SeedModeRatings(GameMode.Medium, 2);
        }

        private static void SeedModeRatings(GameMode mode, int lastRound)
        {
            for (int roundNumber = 1; roundNumber <= lastRound; roundNumber++)
            {
                var rating = (RoundRating)Random.Range(
                    (int)RoundRating.Bad,
                    (int)RoundRating.Good + 1);

                YG2.saves.RoundRatings.Save(mode, roundNumber, rating, keepBest: false);
            }
        }
    }
}
#endif