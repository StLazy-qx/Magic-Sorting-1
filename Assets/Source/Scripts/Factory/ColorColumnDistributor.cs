using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.Pool;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using Assets.Source.Scripts.Colorize;
using System;
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
        private HashSet<IColorStrategy> _roundColors;

        public bool HasTarget => _isTargetAssigned;
        public Color TargetColor { get; private set; }

        public void Initialize(
            ColorRecipes colorRecipes,
            ColorConverter colorConverter,
            IEnumerable<IColorStrategy> roundColors,
            int countMixColors)
        {
            if (colorRecipes == null)
                throw new ArgumentNullException(nameof(colorRecipes));
            if (colorConverter == null)
                throw new ArgumentNullException(nameof(colorConverter));
            if (roundColors == null)
                throw new ArgumentNullException(nameof(roundColors));
            if (countMixColors < 0)
                throw new ArgumentOutOfRangeException(nameof(countMixColors));

            _countMixColors  = countMixColors;
            _colorConverter  = colorConverter;
            _roundColors     = new HashSet<IColorStrategy>(roundColors);
            _colorRecipes    = colorRecipes;
            TargetColor      = default;
            _isTargetAssigned = false;
        }

        public void Distribute()
        {
            Validate();

            while (TryGetRandomColumnWithSpace(out MagicColumn column))
            {
                ColorEntry colorEntry = _colorPool.Get();
                if (colorEntry == null) break;

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
            => _countMixColors > 0 && Random.value < MixChance;

        private bool TryMixColor(
            Color color,
            out Color firstColor,
            out Color secondColor)
        {
            firstColor  = default;
            secondColor = default;

            if (ShouldMixColor() == false)
                return false;

            IColorStrategy strategyColor = _colorConverter.TransformColorToStrategy(color);

            if (strategyColor == null)
                return false;

            if (_colorRecipes.TryGetPair(strategyColor, out var pair) == false)
                return false;

            if (_roundColors.Contains(pair.First) == false
                || _roundColors.Contains(pair.Second) == false)
                return false;

            firstColor  = _colorConverter.TransformStrategyToColor(pair.First);
            secondColor = _colorConverter.TransformStrategyToColor(pair.Second);

            if (_isTargetAssigned == false)
            {
                TargetColor = _colorConverter.TransformStrategyToColor(strategyColor);

                if (_colorMixView != null)
                    _colorMixView.Apply(TargetColor, _countMixColors);

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
                .Where(c => c.CanAddCell())
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