using System;
using System.Collections.Generic;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Player;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class RoundRatingSection : BaseSection
    {
        private readonly Dictionary<(GameMode mode, int roundIndex), RoundRating> _ratings = new();

        public RoundRatingSection(SavesYG saves) : base(saves)
        {
            RebuildCache();
        }

        public IReadOnlyDictionary<(GameMode mode, int roundIndex), RoundRating> Ratings => _ratings;

        public bool TryGet(GameMode mode, int roundIndex, out RoundRating rating)
            => _ratings.TryGetValue((mode, roundIndex), out rating);

        public RoundRating Get(GameMode mode, int roundIndex)
            => _ratings.TryGetValue((mode, roundIndex), out RoundRating rating)
                ? rating
                : RoundRating.NotCompleted;

        public void Save(GameMode mode, int roundIndex, RoundRating rating, bool keepBest = true)
        {
            if (IsSaveableMode(mode) == false)
                return;

            if (roundIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(roundIndex));

            var key = (mode, roundIndex);

            if (keepBest
                && _ratings.TryGetValue(key, out RoundRating existingRating)
                && existingRating >= rating)
                return;

            _ratings[key] = rating;
            PersistEntry(mode, roundIndex, rating);
            SaveProgress();
        }

        public Dictionary<int, RoundRating> GetRatingsForMode(GameMode mode)
        {
            var result = new Dictionary<int, RoundRating>();

            foreach (var ratingPair in _ratings)
            {
                if (ratingPair.Key.mode == mode)
                    result[ratingPair.Key.roundIndex] = ratingPair.Value;
            }

            return result;
        }

        public bool IsSaveableMode(GameMode mode)
            => mode == GameMode.Easy
            || mode == GameMode.Medium
            || mode == GameMode.Hard;

        private void RebuildCache()
        {
            _ratings.Clear();

            var savedEntries = _saves.RoundRatingEntries;
            if (savedEntries == null)
                return;

            for (int index = 0; index < savedEntries.Count; index++)
            {
                var savedEntry = savedEntries[index];
                var mode = (GameMode)savedEntry.GameMode;

                if (IsSaveableMode(mode) == false)
                    continue;

                var key = (mode, savedEntry.RoundIndex);
                var rating = (RoundRating)savedEntry.Rating;

                if (_ratings.TryGetValue(key, out RoundRating currentRating) == false
                    || rating > currentRating)
                {
                    _ratings[key] = rating;
                }
            }
        }

        private void PersistEntry(GameMode mode, int roundIndex, RoundRating rating)
        {
            var savedEntries = _saves.RoundRatingEntries;

            for (int index = 0; index < savedEntries.Count; index++)
            {
                var savedEntry = savedEntries[index];

                if (savedEntry.GameMode == (int)mode && savedEntry.RoundIndex == roundIndex)
                {
                    savedEntry.Rating = (int)rating;
                    savedEntries[index] = savedEntry;
                    return;
                }
            }

            savedEntries.Add(new RoundRatingEntry((int)mode, roundIndex, (int)rating));
        }
    }
}