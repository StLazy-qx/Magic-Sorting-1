using System.Collections.Generic;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Player;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class RoundSequenceProvider
    {
        public const int RoundsPerMode = 6;
        public const int TutorialRoundsCount = 3;
        public const int FreeModeBatchSize = 50;

        public int EasyStartRoundNumber => 1;
        public int EasyEndRoundNumber => EasyStartRoundNumber + RoundsPerMode - 1;
        public int MediumStartRoundNumber => EasyEndRoundNumber + 1;
        public int MediumEndRoundNumber => MediumStartRoundNumber + RoundsPerMode - 1;
        public int HardStartRoundNumber => MediumEndRoundNumber + 1;
        public int HardEndRoundNumber => HardStartRoundNumber + RoundsPerMode - 1;

        public IReadOnlyList<int> GetSequence(GameMode mode, int fallbackRoundNumber)
        {
            switch (mode)
            {
                case GameMode.Easy: 
                    return GetEasy();
                case GameMode.Medium: 
                    return GetMedium();
                case GameMode.Hard: 
                    return GetHard();
                case GameMode.Tutorial: 
                    return GetTutorial();
                case GameMode.FreeMode: 
                    return GetFreeMode(fallbackRoundNumber, FreeModeBatchSize);
                default: 
                    return GetTutorial();
            }
        }

        public IReadOnlyList<int> GetEasy()
            => BuildRange(EasyStartRoundNumber, EasyEndRoundNumber);

        public IReadOnlyList<int> GetMedium()
            => BuildRange(MediumStartRoundNumber, MediumEndRoundNumber);

        public IReadOnlyList<int> GetHard()
            => BuildRange(HardStartRoundNumber, HardEndRoundNumber);

        public IReadOnlyList<int> GetTutorial()
            => BuildRange(1, TutorialRoundsCount);

        public IReadOnlyList<int> GetFreeMode(int startRoundNumber, int batchSize)
        {
            Guard.Positive(batchSize, nameof(batchSize));

            int safeStart = startRoundNumber > 0 ? startRoundNumber : 1;
            return BuildRange(safeStart, safeStart + batchSize - 1);
        }

        // ------------------------------------------------------------------
        // Определение режима и стадии по сохранённому номеру раунда
        // ------------------------------------------------------------------

        public GameMode DetermineModeByRoundNumber(int savedRoundNumber)
        {
            if (savedRoundNumber <= EasyEndRoundNumber) return GameMode.Easy;
            if (savedRoundNumber <= MediumEndRoundNumber) return GameMode.Medium;
            if (savedRoundNumber <= HardEndRoundNumber) return GameMode.Hard;
            return GameMode.FreeMode;
        }

        public int GetStartRoundNumber(GameMode mode) => mode switch
        {
            GameMode.Easy => EasyStartRoundNumber,
            GameMode.Medium => MediumStartRoundNumber,
            GameMode.Hard => HardStartRoundNumber,
            _ => 1
        };

        public int GetEndRoundNumber(GameMode mode) => mode switch
        {
            GameMode.Easy => EasyEndRoundNumber,
            GameMode.Medium => MediumEndRoundNumber,
            GameMode.Hard => HardEndRoundNumber,
            _ => 0
        };

        public int GetTotalRoundsInMode(GameMode mode)
        {
            int start = GetStartRoundNumber(mode);
            int end = GetEndRoundNumber(mode);

            return end >= start ? end - start + 1 : 0;
        }

        // 0 — игрок ещё не дошёл; total — режим пройден.
        public int GetCurrentRoundInMode(int savedRoundNumber, GameMode mode)
        {
            int start = GetStartRoundNumber(mode);
            int end = GetEndRoundNumber(mode);
            int total = GetTotalRoundsInMode(mode);

            if (savedRoundNumber < start) return 0;
            if (savedRoundNumber > end) return total;

            return savedRoundNumber - start + 1;
        }

        public int GetCompletedRoundsInMode(int savedRoundNumber, GameMode mode)
        {
            int start = GetStartRoundNumber(mode);
            int end = GetEndRoundNumber(mode);
            int total = GetTotalRoundsInMode(mode);

            if (savedRoundNumber <= start) return 0;
            if (savedRoundNumber > end) return total;

            return savedRoundNumber - start;
        }

        public string GetProgressText(int savedRoundNumber, GameMode mode)
        {
            int current = GetCurrentRoundInMode(savedRoundNumber, mode);
            int total = GetTotalRoundsInMode(mode);

            return $"{current}/{total}";
        }

        private static IReadOnlyList<int> BuildRange(int startRoundNumber, int endRoundNumber)
        {
            Guard.Positive(startRoundNumber, nameof(startRoundNumber));
            Guard.Positive(endRoundNumber, nameof(endRoundNumber));
            Guard.IsTrue(endRoundNumber >= startRoundNumber,
                nameof(endRoundNumber),
                "Конец диапазона не может быть меньше начала.");

            int count = endRoundNumber - startRoundNumber + 1;
            var result = new List<int>(count);

            for (int roundNumber = startRoundNumber; roundNumber <= endRoundNumber; roundNumber++)
                result.Add(roundNumber);

            return result;
        }
    }
}