using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.SceneManagement;
using Assets.Source.Scripts.UI.Buttons;
using Assets.Source.Scripts.UI.Buttons.Main_Mode_Panel;
using Assets.Source.Scripts.UI.GameModeView;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.Game_Progress
{
    public class MainMenuPanelsPresenter : MonoBehaviour
    {
        private const int DefaultIndexMode = 0;

        [SerializeField] private ClickButton[] _gameModeButtons;
        [SerializeField] private ClickButton _buttonLeftNavigation;
        [SerializeField] private ClickButton _buttonRightNavigation;
        [SerializeField] private GameResumerButton _playButton;
        [SerializeField] private ModeRatingView _ratingView;

        private SceneLoader _sceneLoader;
        private PlayerProgress _playerProgress;
        private GameModePanel[] _gameModePanels;
        private int _currentModeIndex;
        private bool _isStarted;

        private GameModePanel CurrentPanel => _gameModePanels[_currentModeIndex];

        private void Awake()
        {
            Guard.NotNullOrEmpty(_gameModeButtons, nameof(_gameModeButtons));
            Guard.NotNull(_buttonLeftNavigation, nameof(_buttonLeftNavigation));
            Guard.NotNull(_buttonRightNavigation, nameof(_buttonRightNavigation));
            Guard.NotNull(_playButton, nameof(_playButton));

            _gameModePanels = new GameModePanel[_gameModeButtons.Length];

            for (int i = 0; i < _gameModeButtons.Length; i++)
            {
                _gameModePanels[i] = _gameModeButtons[i].GetComponent<GameModePanel>();
                int index = i;
                _gameModeButtons[i].Clicked += () => OnModeButtonClicked(index);
            }

            _buttonLeftNavigation.Clicked += OnChangeLeftView;
            _buttonRightNavigation.Clicked += OnChangeRightView;
            _currentModeIndex = DefaultIndexMode;

            UpdateView();
        }

        private void Start()
        {
            _isStarted = true;

            SelectSavedModePanel();
            RefreshModeAvailability();
            UpdateView();
        }

        private void OnDestroy()
        {
            if (_buttonLeftNavigation != null)
                _buttonLeftNavigation.Clicked -= OnChangeLeftView;

            if (_buttonRightNavigation != null)
                _buttonRightNavigation.Clicked -= OnChangeRightView;
        }

        [Inject]
        public void Construct(SceneLoader sceneLoader, PlayerProgress playerProgress)
        {
            _sceneLoader = sceneLoader;
            _playerProgress = playerProgress;
        }

        public void Initialize()
        {
            if (_isStarted == false)
                return;

            RefreshModeAvailability();
            UpdatePlayButton();
            UpdateRatingView();
        }

        private void RefreshModeAvailability()
        {
            foreach (GameModePanel panel in _gameModePanels)
                panel.SetAvailable(_playerProgress.IsModeAvailable(panel.Mode));
        }

        private void UpdateRatingView()
        {
            if (_ratingView == null || _isStarted == false)
                return;

            if (_playerProgress.TryGetModeRating(CurrentPanel.Mode, out RoundRating rating))
                _ratingView.Show(rating);
            else
                _ratingView.Hide();
        }

        private void SelectSavedModePanel()
        {
            GameMode savedMode = _playerProgress.CurrentMode;

            for (int i = 0; i < _gameModePanels.Length; i++)
            {
                if (_gameModePanels[i].Mode == savedMode)
                {
                    _currentModeIndex = i;

                    return;
                }
            }

            _currentModeIndex = DefaultIndexMode;
        }

        private void UpdatePlayButton()
        {
            if (_isStarted == false)
                return;

            _playButton.SetInteractable(_playerProgress.IsModeAvailable(CurrentPanel.Mode));
        }

        private void OnModeButtonClicked(int index)
        {
            GameModePanel panel = _gameModePanels[index];

            if (_playerProgress.IsModeAvailable(panel.Mode) == false)
                return;

            _playerProgress.SaveSelectedMode(panel.Mode);
            _sceneLoader.LoadScene(panel.SceneIndex);
        }

        private void OnChangeLeftView()
        {
            _currentModeIndex--;

            if (_currentModeIndex < 0)
                _currentModeIndex = _gameModeButtons.Length - 1;

            UpdateView();
        }

        private void OnChangeRightView()
        {
            _currentModeIndex++;

            if (_currentModeIndex >= _gameModeButtons.Length)
                _currentModeIndex = 0;

            UpdateView();
        }

        private void UpdateView()
        {
            for (int i = 0; i < _gameModeButtons.Length; i++)
                _gameModeButtons[i].gameObject.SetActive(i == _currentModeIndex);

            UpdatePlayButton();
            UpdateRatingView();
        }
    }
}