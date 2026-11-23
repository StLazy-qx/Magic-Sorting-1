using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;

namespace Assets.Source.Scripts.Game_Progress
{
    public static class RoundRankCalculator
    {
        private const float BadThreshold = 0.3f;
        private const float MediumThreshold = 0.7f;
        private const int MaxScorePerVessel = 50;

        //переписать не статик
        public static RoundRating Calculate(int earnedScore, int vesselCount)
        {
            Guard.Positive(vesselCount, nameof(vesselCount));

            float percent = (float)earnedScore / (vesselCount * MaxScorePerVessel);

            if (percent < BadThreshold)
                return RoundRating.Bad;

            if (percent < MediumThreshold)
                return RoundRating.Medium;

            return RoundRating.Good;
        }
    }
}