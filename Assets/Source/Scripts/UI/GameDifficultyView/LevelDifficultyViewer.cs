using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.GameDifficulty;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Source.Scripts.UI.GameDifficultyView
{
    public class LevelDifficultyViewer : MonoBehaviour
    {
        [SerializeField] private Button _easyLevelButton;
        [SerializeField] private Button _middleLevelButton;
        [SerializeField] private Button _hardLevelButton;
        [SerializeField] private Color _selectedColor;

        private DifficultyState _difficultyState;
        private Color _defaultColorButton;

        private void Awake()
        {
            ValidateObjects();
            DefineDefaultColor();
        }

        private void Start()
        {
            OnDifficultyChanged(_difficultyState.CurrentDifficulty);
        }

        private void OnEnable()
        {
            _easyLevelButton.onClick.AddListener(SetEasy);
            _middleLevelButton.onClick.AddListener(SetMedium);
            _hardLevelButton.onClick.AddListener(SetHard);

            _difficultyState.DifficultyChanged += OnDifficultyChanged;
        }

        private void OnDisable()
        {
            _easyLevelButton.onClick.RemoveListener(SetEasy);
            _middleLevelButton.onClick.RemoveListener(SetMedium);
            _hardLevelButton.onClick.RemoveListener(SetHard);

            _difficultyState.DifficultyChanged -= OnDifficultyChanged;
        }

        [Inject]
        public void Construct(DifficultyState state)
        {
            _difficultyState = state;
        }

        private void SetEasy()
            => _difficultyState.SetDifficulty(DifficultyLevel.Easy);

        private void SetMedium()
            => _difficultyState.SetDifficulty(DifficultyLevel.Medium);

        private void SetHard()
            => _difficultyState.SetDifficulty(DifficultyLevel.Hard);

        private void OnDifficultyChanged(DifficultyLevel level)
        {
            switch (level)
            {
                case DifficultyLevel.Easy:
                    HighlightButton(_easyLevelButton);
                    break;

                case DifficultyLevel.Medium:
                    HighlightButton(_middleLevelButton);
                    break;

                case DifficultyLevel.Hard:
                    HighlightButton(_hardLevelButton);
                    break;
            }
        }

        private void HighlightButton(Button button)
        {
            ResetButtonColors();

            button.image.color = _selectedColor;
        }

        private void DefineDefaultColor()
        {
            _defaultColorButton = _easyLevelButton.image.color;
        }

        private void ResetButtonColors()
        {
            _easyLevelButton.image.color = _defaultColorButton;
            _middleLevelButton.image.color = _defaultColorButton;
            _hardLevelButton.image.color = _defaultColorButton;
        }

        private void ValidateObjects()
        {
            if (_easyLevelButton == null)
                throw new ArgumentNullException(nameof(_easyLevelButton));

            if (_middleLevelButton == null)
                throw new ArgumentNullException(nameof(_middleLevelButton));

            if (_hardLevelButton == null)
                throw new ArgumentNullException(nameof(_hardLevelButton));
        }
    }
}