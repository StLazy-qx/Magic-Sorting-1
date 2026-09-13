using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.Extensions;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Assets.Source.Scripts.UI.RoundView
{
    public class CountRoundView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _mainText;
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private LevelCounter _levelCounter;
        [SerializeField] private GameSessionHandler _sessionHandler;

        private void Awake()
        {
            Guard.NotNull(_mainText, nameof(_mainText));
            Guard.NotNull(_countText, nameof(_countText));
            Guard.NotNull(_levelCounter, nameof(_levelCounter));
            Guard.NotNull(_sessionHandler, nameof(_sessionHandler));

            _countText.text = _levelCounter.RoundNumber.ToString();
        }

        private void Start()
        {
            OnTextFade();
        }

        private void OnEnable()
        {
            _levelCounter.RoundChanged += OnRoundNumberChanged;
            _sessionHandler.GameLaunching += OnTextFade;
        }

        private void OnDisable()
        {
            _levelCounter.RoundChanged -= OnRoundNumberChanged;
            _sessionHandler.GameLaunching -= OnTextFade;
        }

        public void OnTextFade()
        {
            FadeText(_mainText);
            FadeText(_countText);
        }

        private void FadeText(TMP_Text text)
        {
            const float InitialAlpha = 1f;
            const float TargetAlpha = 0f;
            const float FadeDuration = 2.5f;
            const float FadeDelay = 0.5f;

            text.DOKill();

            text.alpha = InitialAlpha;
            text.DOFade(TargetAlpha, FadeDuration)
                .SetDelay(FadeDelay)
                .SetEase(Ease.OutQuad);
        }

        private void OnRoundNumberChanged(int roundNumber)
        {
            Debug.Log("Номер раунда - " + roundNumber);

            _countText.text = roundNumber.ToString();
        }
    }
}
