using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Extensions;
using UnityEngine;

namespace Assets.Source.Scripts.InteractiveObjects
{
    public class MixerState
    {
        private readonly ColorRecipes _colorRecipes;
        private readonly ColorConverter _colorConverter;

        private IColorStrategy _firstColor;
        private bool _hasMixedResult;

        public MixerState(ColorRecipes colorRecipes, ColorConverter colorConverter)
        {
            Guard.NotNull(colorRecipes, nameof(colorRecipes));
            Guard.NotNull(colorConverter, nameof(colorConverter));

            _colorRecipes = colorRecipes;
            _colorConverter = colorConverter;
        }

        public bool HasFirstIngredient => _firstColor != null;
        public bool HasMixedResult => _hasMixedResult;
        public bool CanAcceptCell => _hasMixedResult == false;

        public bool CanAcceptCellColor(Color color)
        {
            if (CanAcceptCell == false)
                return false;

            IColorStrategy newColor = _colorConverter.TransformColorToStrategy(color);

            if (_firstColor == null)
                return _colorRecipes.CanBeIngredient(newColor);

            return _colorRecipes.CanMix(_firstColor, newColor);
        }

        public void AcceptFirst(Color color)
        {
            _firstColor = _colorConverter.TransformColorToStrategy(color);
        }

        public bool TryMix(Color secondColor, out Color resultColor)
        {
            resultColor = default;

            if (_firstColor == null)
                return false;

            IColorStrategy second = _colorConverter.TransformColorToStrategy(secondColor);

            if (_firstColor == second
                || _colorRecipes.TryGetResult(
                    _firstColor,
                    second,
                    out IColorStrategy resultStrategy) == false)
            {
                return false;
            }

            resultColor = _colorConverter.TransformStrategyToColor(resultStrategy);
            _firstColor = null;
            _hasMixedResult = true;

            return true;
        }

        public void Reset()
        {
            _firstColor = null;
            _hasMixedResult = false;
        }
    }
}