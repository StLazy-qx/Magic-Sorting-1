using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.SceneManagement;
using Assets.Source.Scripts.UI.Buttons;
using Assets.Source.Scripts.UI.Buttons.Main_Mode_Panel;
using System;
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
            if (_gameModeButtons == null || _gameModeButtons.Length == 0)
                throw new InvalidOperationException("Game mode buttons are not assigned.");

            if (_buttonLeftNavigation == null)
                throw new InvalidOperationException("Left navigation button is not assigned.");

            if (_buttonRightNavigation == null)
                throw new InvalidOperationException("Right navigation button is not assigned.");

            if (_playButton == null)
                throw new InvalidOperationException("Play button is not assigned.");

            _gameModePanels = new GameModePanel[_gameModeButtons.Length];

            for (int i = 0; i < _gameModeButtons.Length; i++)
            {
                GameModePanel panel = _gameModeButtons[i].GetComponent<GameModePanel>();

                if (panel == null)
                    throw new MissingComponentException(
                        $"GameModePanel is missing on mode button '{_gameModeButtons[i].name}'.");

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
            if (_playerProgress == null)
                throw new InvalidOperationException(
                    $"{nameof(PlayerProgress)} is not set. Call {nameof(Initialize)} before Start.");

            if (_playerProgress.IsInitialized == false)
                throw new InvalidOperationException(
                    $"{nameof(PlayerProgress)} is not initialized. Call Initialize() before Start.");

            _isStarted = true;

            RefreshModeAvailability();
            UpdatePlayButton();
        }

        [Inject]
        public void Construct(SceneLoader sceneLoader/*, PlayerProgress playerProgress*/)
        {
            _sceneLoader = sceneLoader ?? throw new ArgumentNullException(nameof(sceneLoader));
            //_playerProgress = playerProgress ?? throw new ArgumentNullException(nameof(playerProgress));
        }

        public void Initialize(PlayerProgress playerProgress)
        {
            _playerProgress = playerProgress ?? throw new ArgumentNullException(nameof(playerProgress));

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
                bool isAvailable = _playerProgress.IsModeAvailable(panel.Mode);

                panel.SetAvailable(isAvailable);
            }
        }

        private void UpdatePlayButton()
        {
            if (_isStarted == false)
                return;

            GameModePanel panel = CurrentPanel;
            bool isAvailable = _playerProgress.IsModeAvailable(panel.Mode);

            _playButton.SetInteractable(isAvailable);

            Debug.Log($"[MainMenu] Кнопка Play для режима {panel.Mode}: {(isAvailable ? "доступна" : "недоступна")}");
        }

        private void OnModeButtonClicked(int index)
        {
            GameModePanel panel = _gameModePanels[index];

            Debug.Log($"[MainMenu] Клик по кнопке режима. Выбран уровень сложности: {panel.Mode}");

            if (_playerProgress == null || _sceneLoader == null)
                return;

            if (_playerProgress.IsModeAvailable(panel.Mode) == false)
            {
                Debug.Log($"[MainMenu] Режим {panel.Mode} недоступен");

                return;
            }

            _sceneLoader.LoadScene(panel.SceneIndex);
        }

        private void OnChangeLeftView()
        {
            _currentModeIndex--;

            if (_currentModeIndex < 0)
                _currentModeIndex = _gameModeButtons.Length - 1;

            UpdateView();
            Debug.Log($"[MainMenu] Стрелка влево. Текущий режим: {CurrentPanel.Mode}");
        }

        private void OnChangeRightView()
        {
            _currentModeIndex++;

            if (_currentModeIndex >= _gameModeButtons.Length)
                _currentModeIndex = 0;

            UpdateView();
            Debug.Log($"[MainMenu] Стрелка вправо. Текущий режим: {CurrentPanel.Mode}");
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