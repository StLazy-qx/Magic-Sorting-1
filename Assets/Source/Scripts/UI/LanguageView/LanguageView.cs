using UnityEngine;
using UnityEngine.UI;
using Assets.Source.Scripts.Language;
using System;

namespace Assets.Source.Scripts.UI.LanguageView
{
    public class LanguageView : MonoBehaviour
    {
        private const string RussianLanguage = "ru";
        private const string EnglishLanguage = "en";
        private const string TurkishLanguage = "tr";

        [SerializeField] private Button _englishButton;
        [SerializeField] private Button _russianButton;
        [SerializeField] private Button _turkishButton;
        [SerializeField] private Color _selectedColor;

        // сделать посредник между логикой _languageSetter и UI языка
        private Color _defaultColorButton;
        private LanguageSetter _languageSetter;

        public void Initialize(LanguageSetter setter)
        {
            ValidateInitializeArguments(setter);

            _languageSetter = setter;
            _defaultColorButton = _englishButton.image.color;
            _languageSetter.OnLanguageChanged += UpdateUI;

            UpdateUI(_languageSetter.CurrentLanguage);
        }

        private void OnEnable()
        {
            _russianButton.onClick.AddListener(() => _languageSetter.SetLanguage(RussianLanguage));
            _englishButton.onClick.AddListener(() => _languageSetter.SetLanguage(EnglishLanguage));
            _turkishButton.onClick.AddListener(() => _languageSetter.SetLanguage(TurkishLanguage));
        }

        private void OnDisable()
        {
            ValidateButtons();
            _russianButton.onClick.RemoveAllListeners();
            _englishButton.onClick.RemoveAllListeners();
            _turkishButton.onClick.RemoveAllListeners();
        }

        private void UpdateUI(string langCode)
        {
            ResetButtonColors();

            switch (langCode)
            {
                case RussianLanguage:
                    Highlight(_russianButton);
                    break;

                case EnglishLanguage:
                    Highlight(_englishButton);
                    break;

                case TurkishLanguage:
                    Highlight(_turkishButton);
                    break;

                default:
                    throw new ArgumentException(
                        $"Unsupported language code: {langCode}", nameof(langCode));

            }
        }

        private void Highlight(Button button)
        {
            if (button == null)
            {
                throw new NullReferenceException(
                    "Button reference is missing when trying to highlight.");
            }

            button.image.color = _selectedColor;
        }

        private void ResetButtonColors()
        {
            _russianButton.image.color = _defaultColorButton;
            _englishButton.image.color = _defaultColorButton;
            _turkishButton.image.color = _defaultColorButton;
        }

        private void ValidateInitializeArguments(LanguageSetter setter)
        {
            if (setter == null)
            {
                throw new ArgumentNullException(nameof(setter),
                    "LanguageSetter reference cannot be null.");
            }

            ValidateButtons();

            if (_selectedColor == default)
            {
                throw new InvalidOperationException(
                    "Selected color is not set in the inspector.");
            }
        }

        private void ValidateButtons()
        {
            if (_russianButton == null)
            {
                throw new NullReferenceException(
                    "Russian button reference is missing in LanguageView.");
            }

            if (_englishButton == null)
            {
                throw new NullReferenceException(
                    "English button reference is missing in LanguageView.");
            }

            if (_turkishButton == null)
            {
                throw new NullReferenceException(
                    "Turkish button reference is missing in LanguageView.");
            }
        }
    }
}