using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.UI.Buttons;
using UnityEngine;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class RewardedAdvertisementHandler : MonoBehaviour 
    {
        private const int FirstRoundNumber = 1;

        [SerializeField] private LevelCounter _levelCounter;
        [SerializeField] private IconRewardedAdvertisement _iconView;

        private void Awake()
        {
            Guard.NotNull(_levelCounter, nameof(_levelCounter));
            Guard.NotNull(_iconView, nameof(_iconView));

            _iconView.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _levelCounter.RoundChanged += OnShowIconReward;
        }

        private void OnDisable()
        {
            _levelCounter.RoundChanged -= OnShowIconReward;
        }

        private void OnShowIconReward(int value)
        {
            Guard.NotNull(value, nameof(value));

            if (_levelCounter.RoundNumber > FirstRoundNumber)
            {
                _iconView.gameObject.SetActive(true);
            }
        }
    }
}
