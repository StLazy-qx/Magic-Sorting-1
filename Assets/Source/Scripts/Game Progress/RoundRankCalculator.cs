using Assets.Source.Scripts.Enums;

namespace Assets.Source.Scripts.Game_Progress
{
    public class RoundRankCalculator
    {
        private const float BadThreshold = 0.3f; 
        private const float MediumThreshold = 0.7f;

        private int _countVessels;
        private int _maxScore;

        public RoundRankCalculator(int countVessels)
        {
            _countVessels = countVessels;

            _maxScore = _countVessels * 50;
        }

        public RoundRating Calculate(int earnedScore)
        {
            float percent = (float)earnedScore / _maxScore;

            if (percent < BadThreshold) 
                return RoundRating.Bad;

            if (percent < MediumThreshold) 
                return RoundRating.Medium;

            return RoundRating.Good;
        }
    }
}
