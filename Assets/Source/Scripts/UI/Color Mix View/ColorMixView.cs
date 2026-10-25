using Assets.Source.Scripts.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.UI.Color_Mix_View
{
    public class ColorMixView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private Image _colorImage;

        private bool _hasColor;
        private bool _hasCount;
        private int _count;
        private Color _color;

        private void Awake()
        {
            Guard.NotNull(_countText, nameof(_countText));
            Guard.NotNull(_colorImage, nameof(_colorImage));
        }

        public void Apply(Color color, int count)
        {
            ClearState();
            Guard.Positive(count, nameof(count));

            _color = color;
            _count = count;
            _hasColor = true;
            _hasCount = true;

            TryApply();
        }

        private void ClearState()
        {
            _hasColor = false;
            _hasCount = false;
        }

        private void TryApply()
        {
            if (_hasColor == false ||
                _hasCount == false)
                return;

            Guard.NotNull(_colorImage, nameof(_colorImage));
            Guard.NotNull(_countText, nameof(_countText));

            _colorImage.color = _color;
            _countText.text = _count.ToString();
        }
    }
}
