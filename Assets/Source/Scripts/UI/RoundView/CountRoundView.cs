using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Player;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Zenject;

namespace Assets.Source.Scripts.UI.RoundView
{
    public class CountRoundView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _mainText;
        [SerializeField] private TMP_Text _countText;
        //[SerializeField] private TMP_Text _modeText;
        [SerializeField] private LevelCounter _levelCounter;
        [SerializeField] private BaseGameHandler _gameHandler;

        private PlayerProgress _playerProgress;

        [Inject]
        private void Construct(PlayerProgress playerProgress)
        {
            Guard.NotNull(playerProgress, nameof(playerProgress));

            _playerProgress = playerProgress;
        }

        private void Awake()
        {
            Guard.NotNull(_mainText, nameof(_mainText));
            Guard.NotNull(_countText, nameof(_countText));
            //Guard.NotNull(_modeText, nameof(_modeText));
            Guard.NotNull(_levelCounter, nameof(_levelCounter));
            Guard.NotNull(_gameHandler, nameof(_gameHandler));
            Guard.NotNull(_playerProgress, nameof(_playerProgress));

            Refresh();
        }

        private void Start() => OnTextFade();

        private void OnEnable()
        {
            _levelCounter.RoundChanged += Refresh;

            if (_gameHandler is GameSessionHandler sessionHandler)
                sessionHandler.GameLaunching += OnRoundStarted;
        }

        private void OnDisable()
        {
            _levelCounter.RoundChanged -= Refresh;

            if (_gameHandler is GameSessionHandler sessionHandler)
                sessionHandler.GameLaunching -= OnRoundStarted;
        }

        private void OnRoundStarted()
        {
            Refresh();
            OnTextFade();
        }

        private void OnTextFade()
        {
            FadeText(_mainText);
            FadeText(_countText);
        }

        private void Refresh()
        {
            //_modeText.text = GetModeTitle(_playerProgress.CurrentMode);
            _countText.text = _levelCounter.RoundNumber.ToString();
        }

        //private static string GetModeTitle(GameMode mode) => mode switch
        //{
        //    GameMode.Tutorial => "Tutorial",
        //    GameMode.Easy => "Easy",
        //    GameMode.Medium => "Medium",
        //    GameMode.Hard => "Hard",
        //    GameMode.FreeMode => "Free Mode",
        //    _ => string.Empty
        //};

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

        private void OnRoundNumberChanged()
        {
            _countText.text = _levelCounter.RoundNumber.ToString();
        }
    }
}
