using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.SceneManagement;
using Assets.Source.Scripts.UI.Buttons;
using Assets.Source.Scripts.UI.Buttons.Main_Mode_Panel;
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
                GameModePanel panel = _gameModeButtons[i].GetComponent<GameModePanel>();

                Guard.NotNull(panel, $"{nameof(_gameModeButtons)}[{i}].GameModePanel");

                _gameModePanels[i] = panel;

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

        //private void Start()
        //{
        //    _isStarted = true;

        //    RefreshModeAvailability();
        //    UpdatePlayButton();
        //}

        [Inject]
        public void Construct(SceneLoader sceneLoader, PlayerProgress playerProgress)
        {
            Guard.NotNull(sceneLoader, nameof(sceneLoader));
            Guard.NotNull(playerProgress, nameof(playerProgress));

            _sceneLoader = sceneLoader;
            _playerProgress = playerProgress;
        }

        public void Initialize()
        {
            if (_isStarted)
            {
                RefreshModeAvailability();
                UpdatePlayButton();
            }
        }

        private void RefreshModeAvailability()
        {
            for (int i = 0; i < _gameModePanels.Length; i++)
            {
                GameModePanel panel = _gameModePanels[i];

                panel.SetAvailable(_playerProgress.IsModeAvailable(panel.Mode));
            }
        }


        private void SelectSavedModePanel()
        {
            if (_playerProgress == null)
                return;


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

            GameModePanel panel = CurrentPanel;

            _playButton.SetInteractable(_playerProgress.IsModeAvailable(panel.Mode));
        }

        private void OnModeButtonClicked(int index)
        {
            GameModePanel panel = _gameModePanels[index];

            if (_playerProgress == null || _sceneLoader == null)
                return;

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
        }

        private void OnDestroy()
        {
            if (_buttonLeftNavigation != null)
                _buttonLeftNavigation.Clicked -= OnChangeLeftView;

            if (_buttonRightNavigation != null)
                _buttonRightNavigation.Clicked -= OnChangeRightView;
        }
    }
}