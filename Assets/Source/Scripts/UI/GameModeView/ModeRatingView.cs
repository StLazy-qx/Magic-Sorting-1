using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.Vessels;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.UI.GameModeView
{
    public class ModeRatingView : MonoBehaviour
    {
        [SerializeField] private VesselRoundController _vesselRoundController;
        [SerializeField] private Image[] _stars;

        private void Awake()
        {
            Guard.NotNullOrEmpty(_stars, nameof(_stars));
        }

        private void OnEnable()
        {
            _vesselRoundController.ResultSummarizing += OnShow;
        }

        private void OnDisable()
        {
            _vesselRoundController.ResultSummarizing -= OnShow;
        }

        public void Show(RoundRating rating)
        {
            if (rating == RoundRating.NotCompleted)
            {
                Hide();

                return;
            }

            gameObject.SetActive(true);
            RoundRatingStars.Apply(_stars, rating);
        }

        private void OnShow(RoundRating rating)
        {
            if (rating == RoundRating.NotCompleted)
            {
                Hide();

                return;
            }

            gameObject.SetActive(true);
            RoundRatingStars.Apply(_stars, rating);
        }

        public void Hide()
            => gameObject.SetActive(false);
    }
}