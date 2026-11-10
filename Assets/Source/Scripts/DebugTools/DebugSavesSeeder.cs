using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.Player;
using UnityEngine;
using YG;

namespace Assets.Source.Scripts.DebugTools
{
    public static class DebugSavesSeeder
    {
        public static void SeedRatingsForAllRounds()
        {
            YG2.saves.Round.DebugSeedRoundNumber(1);
            SeedModeRatings(GameMode.Easy, 1, GameModeData.ExpectedRoundsCount);
            SeedModeRatings(GameMode.Medium, 1, 2);
        }

        private static void SeedModeRatings(GameMode mode, int startIndex, int endIndex)
        {
            for (int roundIndex = startIndex; roundIndex <= endIndex; roundIndex++)
            {
                var rating = (RoundRating)Random.Range(
                    (int)RoundRating.Bad,
                    (int)RoundRating.Good + 1);

                YG2.saves.RoundRatings.Save(mode, roundIndex, rating, keepBest: false);
            }
        }
    }
}