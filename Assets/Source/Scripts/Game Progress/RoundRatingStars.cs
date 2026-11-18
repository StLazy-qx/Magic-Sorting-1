using Assets.Source.Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Game_Progress
{
    /// Единое правило "сколько звёзд". Звёзды это любые Behaviour (Image и т.п.): включаются свойством enabled.
    /// Image[] подходит как IReadOnlyList&lt;Behaviour&gt; благодаря ковариантности.
    public static class RoundRatingStars
    {
        public static int GetCount(RoundRating rating)
        {
            return rating switch
            {
                RoundRating.Bad => 1,
                RoundRating.Medium => 2,
                RoundRating.Good => 3,
                _ => 0
            };
        }

        public static void Apply(IReadOnlyList<Behaviour> stars, RoundRating rating)
        {
            int count = GetCount(rating);

            for (int i = 0; i < stars.Count; i++)
            {
                if (stars[i] != null)
                    stars[i].enabled = i < count;
            }
        }
    }
}