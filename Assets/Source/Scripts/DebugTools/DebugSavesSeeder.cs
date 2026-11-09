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
            // Номер раунда больше ни на что не влияет, ставим просто 1
            YG2.saves.Round.DebugSeedRoundNumber(1);

            // Easy: проходим все 6 раундов
            SeedModeRatings(GameMode.Easy, 1, GameModeData.ExpectedRoundsCount);

            // Medium: проходим только 2 раунда (остальные останутся NotCompleted)
            SeedModeRatings(GameMode.Medium, 1, 2);

            // Hard: ничего не проходим (режим останется заблокированным)

            Debug.Log($"[DebugSavesSeeder] Seed complete. Total rating entries = {YG2.saves.RoundRatingEntries.Count}");
        }

        private static void SeedModeRatings(GameMode mode, int startIndex, int endIndex)
        {
            for (int roundIndex = startIndex; roundIndex <= endIndex; roundIndex++)
            {
                var rating = (RoundRating)Random.Range(
                    (int)RoundRating.Bad,
                    (int)RoundRating.Good + 1);

                YG2.saves.SaveRoundRating(mode, roundIndex, rating, keepBest: false);

                Debug.Log($"[DebugSavesSeeder] Seeded {mode} Round {roundIndex} = {rating}");
            }
        }
    }
}