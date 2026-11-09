using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Player;
using System;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Game_Progress
{
    public sealed class GameModeRuntime
    {
        private readonly GameModeData _data;
        private readonly RoundRuntime[] _rounds;

        public GameModeRuntime(GameModeData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _data = data;
            _rounds = new RoundRuntime[data.RoundsCount];

            for (int i = 0; i < data.RoundsCount; i++)
                _rounds[i] = new RoundRuntime(data.Rounds[i]);
        }

        public GameMode Mode => _data.Mode;
        public IReadOnlyList<RoundRuntime> Rounds => _rounds;

        public bool IsCompleted
        {
            get
            {
                if (_rounds.Length == 0) return false;

                for (int i = 0; i < _rounds.Length; i++)
                    if (_rounds[i].IsFinished == false)
                        return false;

                return true;
            }
        }

        /// <summary>
        /// Среднее по пройденным раундам текущей сессии. Если ничего не пройдено — NotCompleted.
        /// </summary>
        public RoundRating Rating
        {
            get
            {
                int sum = 0;
                int count = 0;

                for (int i = 0; i < _rounds.Length; i++)
                {
                    var round = _rounds[i];
                    if (round.IsFinished == false) continue;
                    if (round.Rating == RoundRating.NotCompleted) continue;

                    sum += (int)round.Rating;
                    count++;
                }

                if (count == 0) return RoundRating.NotCompleted;

                double average = (double)sum / count;
                int rounded = (int)Math.Round(average, MidpointRounding.AwayFromZero);

                if (rounded < (int)RoundRating.Bad) rounded = (int)RoundRating.Bad;
                if (rounded > (int)RoundRating.Good) rounded = (int)RoundRating.Good;

                return (RoundRating)rounded;
            }
        }

        public void ResetAll()
        {
            for (int i = 0; i < _rounds.Length; i++)
                _rounds[i].Reset();
        }

        public void MarkRoundFinished(int roundIndex, RoundRating rating, bool keepBest = true)
        {
            int index = roundIndex - 1;

            if (index < 0 || index >= _rounds.Length)
                throw new ArgumentOutOfRangeException(nameof(roundIndex),
                    $"roundIndex должен быть в диапазоне 1..{_rounds.Length}, получено {roundIndex}.");

            _rounds[index].MarkFinished(rating, keepBest);
        }
    }
}