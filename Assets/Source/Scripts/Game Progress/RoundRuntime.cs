using Assets.Source.Scripts.Enums;
using System;

namespace Assets.Source.Scripts.Game_Progress
{
    public sealed class RoundRuntime
    {
        public RoundData Data { get; }
        public bool IsFinished { get; private set; }
        public RoundRating Rating { get; private set; }

        public RoundRuntime(RoundData data)
        {
            Data = data;
            Reset();
        }

        public void Reset()
        {
            IsFinished = false;
            Rating = RoundRating.NotCompleted;
        }

        public void MarkFinished(RoundRating rating, bool keepBest = true)
        {
            if (rating == RoundRating.NotCompleted)
                throw new ArgumentException("Нельзя закончить раунд рейтингом NotCompleted.", nameof(rating));

            if (keepBest && IsFinished && Rating >= rating)
                return;

            IsFinished = true;
            Rating = rating;
        }
    }
}