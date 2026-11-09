using System;
using System.Collections.Generic;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;

namespace Assets.Source.Scripts.Game_Progress
{
    public class ModeRankCalculator
    {
        public RoundRating CalculateAverage(IReadOnlyList<RoundRating> ratings)
        {
            Guard.NotNull(ratings, nameof(ratings));

            int sum = 0;
            int count = 0;

            for (int i = 0; i < ratings.Count; i++)
            {
                if (ratings[i] == RoundRating.NotCompleted)
                    continue;

                sum += (int)ratings[i];
                count++;
            }

            if (count == 0)
                return RoundRating.NotCompleted;

            double average = (double)sum / count;
            int rounded = (int)Math.Round(average, MidpointRounding.AwayFromZero);

            if (rounded < (int)RoundRating.Bad)
                rounded = (int)RoundRating.Bad;

            if (rounded > (int)RoundRating.Good)
                rounded = (int)RoundRating.Good;

            return (RoundRating)rounded;
        }
    }
}