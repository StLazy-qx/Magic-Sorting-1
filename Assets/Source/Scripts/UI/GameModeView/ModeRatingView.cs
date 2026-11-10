using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.UI.GameModeView
{
    public class ModeRatingView : MonoBehaviour
    {
        [SerializeField] private Image[] _stars;

        private void Awake()
        {
            Guard.NotNullOrEmpty(_stars, nameof(_stars));
        }

        public void Show(RoundRating rating)
        {
            if (rating == RoundRating.NotCompleted)
            {
                Hide();

                return;
            }

            gameObject.SetActive(true);

            int starsCount = GetStarsCount(rating);

            for (int i = 0; i < _stars.Length; i++)
            {
                if (_stars[i] == null)
                    continue;

                _stars[i].enabled = i < starsCount;
            }
        }

        public void Hide()
            => gameObject.SetActive(false);

        private int GetStarsCount(RoundRating rating)
        {
            switch (rating)
            {
                case RoundRating.Bad:
                    return 1;

                case RoundRating.Medium:
                    return 2;

                case RoundRating.Good:
                    return 3;

                default:
                    return 0;
            }
        }
    }
}