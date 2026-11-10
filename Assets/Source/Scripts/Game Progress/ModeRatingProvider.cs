using System.Collections.Generic;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.YG;
using YG;

namespace Assets.Source.Scripts.Game_Progress
{
    public class ModeRatingProvider
    {
        private readonly GameMode[] _ratedModes =
        {
            GameMode.Easy,
            GameMode.Medium,
            GameMode.Hard
        };

        private readonly RoundSequenceProvider _sequenceProvider;
        private readonly ModeRankCalculator _calculator = new();
        private readonly Dictionary<GameMode, RoundRating> _modeRatings = new();

        public ModeRatingProvider(RoundSequenceProvider sequenceProvider)
        {
            Guard.NotNull(sequenceProvider, nameof(sequenceProvider));

            _sequenceProvider = sequenceProvider;
        }

        public void Refresh()
        {
            _modeRatings.Clear();

            RoundRatingSection section = YG2.saves.RoundRatings;

            for (int i = 0; i < _ratedModes.Length; i++)
                _modeRatings[_ratedModes[i]] = Calculate(_ratedModes[i]);
        }

        public bool TryGetRating(GameMode mode, out RoundRating rating)
        {
            if (_modeRatings.TryGetValue(mode, out rating) == false)
                return false;

            if (rating == RoundRating.NotCompleted)
                return false;

            return true;
        }

        private RoundRating Calculate(GameMode mode)
        {
            RoundRatingSection section = YG2.saves.RoundRatings;
            int start = _sequenceProvider.GetStartRoundNumber(mode);
            int end = _sequenceProvider.GetEndRoundNumber(mode);
            var ratings = new List<RoundRating>(end - start + 1);

            for (int roundNumber = start; roundNumber <= end; roundNumber++)
            {
                RoundRating rating = section.Get(mode, roundNumber);

                if (rating == RoundRating.NotCompleted)
                    return RoundRating.NotCompleted;

                ratings.Add(rating);
            }

            return _calculator.CalculateAverage(ratings);
        }

        //public bool TryGetRating(GameMode mode, out RoundRating rating)
        //{
        //    if (_modeRatings.TryGetValue(mode, out rating) == false)
        //        return false;

        //    if (rating == RoundRating.NotCompleted)
        //    {
        //        rating = RoundRating.NotCompleted;
        //        return false;
        //    }

        //    return true;
        //}

        //private RoundRating Calculate(GameMode mode)
        //{
        //    RoundRatingSection section = YG2.saves.RoundRatings;

        //    int start = _sequenceProvider.GetStartRoundNumber(mode);
        //    int end = _sequenceProvider.GetEndRoundNumber(mode);

        //    var ratings = new List<RoundRating>(end - start + 1);

        //    for (int roundNumber = start; roundNumber <= end; roundNumber++)
        //        ratings.Add(section.Get(mode, roundNumber));

        //    return _calculator.CalculateAverage(ratings);
        //}
    }
}