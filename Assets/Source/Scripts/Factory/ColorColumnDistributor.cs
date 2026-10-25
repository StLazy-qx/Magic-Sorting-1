using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.Pool;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using Assets.Source.Scripts.Colorize;
using System;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.UI.Color_Mix_View;

namespace Assets.Source.Scripts.Factory
{
    public class ColorColumnDistributor : MonoBehaviour
    {
        private const float MixChance = 0.5f;

        [SerializeField] private MagicColumnPool _columnPool;
        [SerializeField] private EntryListColorPool _colorPool;
        [SerializeField] private ColorMixView _colorMixView;

        private int _countMixColors;
        private bool _isTargetAssigned;
        private ColorRecipes _colorRecipes;
        private ColorConverter _colorConverter;
        private HashSet<EnumColor> _roundColors;

        public bool HasTarget => _isTargetAssigned;
        public Color TargetColor { get; private set; }

        public void Initialize(
            ColorRecipes colorRecipes, 
            ColorConverter colorConverter,
            IEnumerable<EnumColor> roundColors,
            int countMixColors)
        {
            _countMixColors = countMixColors;
            _colorConverter = colorConverter;
            _roundColors = new HashSet<EnumColor>(roundColors);
            _colorRecipes = colorRecipes;
            TargetColor = default;
        }

        public void Distribute()
        {
            Validate();

            while (TryGetRandomColumnWithSpace(out MagicColumn column))
            {
                ColorEntry colorEntry = _colorPool.Get();

                if (colorEntry == null)
                    break;

                Color color = colorEntry.ConsumeColor();

                if (TryMixColor(color, out Color firstColor, out Color secondColor))
                {
                    column.AddCell(firstColor, secondColor);
                    _countMixColors--;
                }
                else
                {
                    column.AddCell(color);
                }
            }
        }

        private bool ShouldMixColor()
        {
            if (_countMixColors <= 0)
                return false;

            return Random.value < MixChance;
        }

        private bool TryMixColor(
            Color color,
            out Color firstColor,
            out Color secondColor)
        {
            firstColor = default;
            secondColor = default;

            if (ShouldMixColor() == false)
                return false;

            EnumColor enumColor = _colorConverter.TransformColorToEnum(color);

            if (_colorRecipes.TryGetPair(enumColor, out var pair) == false)
                return false;

            if (_roundColors.Contains(pair.First) == false
                || _roundColors.Contains(pair.Second) == false)
                return false;

            firstColor = _colorConverter.TransformEnumToColor(pair.First);
            secondColor = _colorConverter.TransformEnumToColor(pair.Second);

            if (_isTargetAssigned == false)
            {
                TargetColor = _colorConverter.TransformEnumToColor(enumColor);

                _colorMixView.Apply(TargetColor, _countMixColors);
                Debug.Log(enumColor);

                _isTargetAssigned = true;
            }

            return true;
        }

        private bool TryGetRandomColumnWithSpace(out MagicColumn column)
        {
            column = null;
            IReadOnlyList<MagicColumn> activeColumns = _columnPool.GetActiveObjects();

            if (activeColumns == null || activeColumns.Count == 0)
                return false;

            List<MagicColumn> available = activeColumns
                .Where(column => column.CanAddCell())
                .ToList();

            if (available.Count == 0)
                return false;

            column = available[Random.Range(0, available.Count)];

            return true;
        }

        private void Validate()
        {
            if (_columnPool == null)
                throw new ArgumentNullException(nameof(_columnPool));

            if (_colorPool == null)
                throw new ArgumentNullException(nameof(_colorPool));
        }
    }
}
