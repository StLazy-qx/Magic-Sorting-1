using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using UnityEngine;

namespace Assets.Source.Scripts.InteractiveObjects
{
    public class MixerState
    {
        private readonly ColorRecipes _colorRecipes;
        private readonly ColorConverter _colorConverter;

        private EnumColor? _firstColor;
        private bool _hasMixedResult;

        public MixerState(ColorRecipes colorRecipes, ColorConverter colorConverter)
        {
            Guard.NotNull(colorRecipes, nameof(colorRecipes));
            Guard.NotNull(colorConverter, nameof(colorConverter));

            _colorRecipes = colorRecipes;
            _colorConverter = colorConverter;
        }

        public bool HasFirstIngredient => _firstColor.HasValue;
        public bool HasMixedResult => _hasMixedResult;
        public bool CanAcceptCell => _hasMixedResult == false;

        public bool CanAcceptCellColor(Color color)
        {
            if (CanAcceptCell == false)
                return false;

            EnumColor newColor = _colorConverter.TransformColorToEnum(color);

            if (_firstColor == null)
                return _colorRecipes.CanBeIngredient(newColor);

            return _colorRecipes.CanMix(_firstColor.Value, newColor);
        }

        public void AcceptFirst(Color color)
        {
            _firstColor = _colorConverter.TransformColorToEnum(color);
        }

        public bool TryMix(Color secondColor, out Color resultColor)
        {
            resultColor = default;

            if (_firstColor == null)
                return false;

            EnumColor second = _colorConverter.TransformColorToEnum(secondColor);

            if (_firstColor.Value == second
                || _colorRecipes.TryGetResult(
                    _firstColor.Value,
                    second,
                    out EnumColor enumResult) == false) 
				{
					return false;
				}
                

            resultColor = _colorConverter.TransformEnumToColor(enumResult);
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