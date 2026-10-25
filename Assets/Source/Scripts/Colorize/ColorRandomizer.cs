using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Enums;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.Colorize
{
    public class ColorRandomizer : MonoBehaviour
    {
        private const int BeginCountColors = 2;
        private const int MinColorsCount = 4;

        private readonly ColorConverter _colorConverter = new ColorConverter();
        private readonly EnumColor[] _availableColors =
            (EnumColor[])Enum.GetValues(typeof(EnumColor));

        private EnumColor[] _allColors;
        private Color[] _beginRoundColors;
        private Color[] _remainingColors;
        private Color _targetColor;
        private ColorRecipes _colorRecipes;

        public IReadOnlyList<EnumColor> RoundColors => _allColors;
        public IReadOnlyList<Color> BeginColors => _beginRoundColors;
        public IReadOnlyList<Color> RemainingColors => _remainingColors;
        public Color TargetColor => _targetColor;

        public void SetDifficultySettings(int colorsNumber)
        {
            Guard.Positive(colorsNumber, nameof(colorsNumber));

            if (_colorRecipes == null)
                throw new InvalidOperationException("ColorRecipes is not set.");

            int count = NormalizeColorsCount(colorsNumber);

            _allColors = BuildRoundColors(count);

            int beginCount = Mathf.Min(BeginCountColors, _allColors.Length);

            _targetColor = _colorConverter.TransformEnumToColor(_allColors[0]);
            _beginRoundColors = ConvertColors(_allColors, 0, beginCount);
            _remainingColors = ConvertColors(
                _allColors,
                beginCount,
                _allColors.Length - beginCount);
        }

        public void SetColorRecipes(ColorRecipes colorRecipes)
        {
            Guard.NotNull(colorRecipes, nameof(colorRecipes));

            _colorRecipes = colorRecipes;
        }

        private int NormalizeColorsCount(int requested)
        {
            int maxCount = _availableColors.Length - _availableColors.Length % 2;
            int count = Mathf.Clamp(requested, MinColorsCount, maxCount);

            return count - count % 2;
        }

        private EnumColor[] BuildRoundColors(int count)
        {
            List<EnumColor> result = new List<EnumColor>(count);
            HashSet<EnumColor> used = new HashSet<EnumColor>();

            EnumColor first = PickRandomRecipeColor();
            (EnumColor second, EnumColor third) = _colorRecipes.GetPair(first);

            AddColor(first, result, used);
            AddColor(second, result, used);
            AddColor(third, result, used);

            while (result.Count < count)
                AddColor(PickNextColor(result, used), result, used);

            return result.ToArray();
        }

        private static void AddColor(
            EnumColor color,
            List<EnumColor> result,
            HashSet<EnumColor> used)
        {
            if (used.Add(color))
                result.Add(color);
        }

        private EnumColor PickNextColor(List<EnumColor> selected, HashSet<EnumColor> used)
        {
            // 1. Результат смешивания двух уже выбранных цветов
            List<EnumColor> candidates = GetMixResults(selected, used);

            // 2. Цвет, который достраивает рецепт уже выбранного цвета
            if (candidates.Count == 0)
                candidates = GetRecipeCompleters(used);

            // 3. Любой неиспользованный
            if (candidates.Count == 0)
                candidates = GetUnusedColors(used);

            return candidates[UnityEngine.Random.Range(0, candidates.Count)];
        }

        private List<EnumColor> GetMixResults(List<EnumColor> selected, HashSet<EnumColor> used)
        {
            List<EnumColor> results = new List<EnumColor>();

            for (int i = 0; i < selected.Count; i++)
            {
                for (int j = i + 1; j < selected.Count; j++)
                {
                    if (_colorRecipes.TryGetResult(selected[i], selected[j], out EnumColor result)
                        && used.Contains(result) == false
                        && results.Contains(result) == false)
                    {
                        results.Add(result);
                    }
                }
            }

            return results;
        }

        private List<EnumColor> GetMixPartners(List<EnumColor> selected, HashSet<EnumColor> used)
        {
            List<EnumColor> partners = new List<EnumColor>();

            foreach (EnumColor color in _availableColors)
            {
                if (used.Contains(color))
                    continue;

                foreach (EnumColor existing in selected)
                {
                    if (_colorRecipes.CanMix(color, existing))
                    {
                        partners.Add(color);
                        break;
                    }
                }
            }

            return partners;
        }

        private List<EnumColor> GetUnusedColors(HashSet<EnumColor> used)
        {
            List<EnumColor> unused = new List<EnumColor>();

            foreach (EnumColor color in _availableColors)
            {
                if (used.Contains(color) == false)
                    unused.Add(color);
            }

            return unused;
        }

        private EnumColor PickRandomRecipeColor()
        {
            List<EnumColor> withRecipe = new List<EnumColor>();

            foreach (EnumColor color in _availableColors)
            {
                if (_colorRecipes.HasRecipe(color))
                    withRecipe.Add(color);
            }

            if (withRecipe.Count == 0)
                throw new InvalidOperationException("ColorRecipes has no recipes.");

            return withRecipe[UnityEngine.Random.Range(0, withRecipe.Count)];
        }

        private List<EnumColor> GetRecipeCompleters(HashSet<EnumColor> used)
        {
            List<EnumColor> result = new List<EnumColor>();

            foreach (var recipe in _colorRecipes.ColorMixtures)
            {
                if (used.Contains(recipe.Key) == false)
                    continue;

                bool hasFirst = used.Contains(recipe.Value.First);
                bool hasSecond = used.Contains(recipe.Value.Second);

                // Оба есть: достраивать нечего. Обоих нет: одним цветом не закрыть.
                if (hasFirst == hasSecond)
                    continue;

                EnumColor missing = hasFirst ? recipe.Value.Second : recipe.Value.First;

                if (result.Contains(missing) == false)
                    result.Add(missing);
            }

            return result;
        }

        private Color[] ConvertColors(
            IReadOnlyList<EnumColor> source,
            int startIndex,
            int length)
        {
            Color[] colors = new Color[length];

            for (int i = 0; i < length; i++)
                colors[i] = _colorConverter.TransformEnumToColor(source[startIndex + i]);

            return colors;
        }
    }
}