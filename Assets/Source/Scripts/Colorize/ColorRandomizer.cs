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

        private readonly ColorConverter _colorConverter = new ColorConverter();

        private EnumColor[] _allColors =
            (EnumColor[])Enum.GetValues(typeof(EnumColor));
        private Color[] _beginRoundColors;
        private Color[] _remainingColors;
        private Color _targetColor;
        private ColorRecipes _colorRecipes;

        public IReadOnlyList<Color> BeginColors => _beginRoundColors;
        public IReadOnlyList<Color> RemainingColors => _remainingColors;
        public Color TargetColor => _targetColor;

        public void SetDifficultySettings(int colorsNumber, int countMixeColor)
        {
            Guard.Positive(colorsNumber, nameof(colorsNumber));

            if (_colorRecipes == null)
                throw new InvalidOperationException("ColorRecipes is not set.");

            colorsNumber = Mathf.Min(colorsNumber, _allColors.Length);

            IReadOnlyList<EnumColor> roundColors =
                BuildRoundColors(colorsNumber, countMixeColor, out EnumColor targetColor);

            int totalColors = roundColors.Count;
            int beginCount = Mathf.Min(BeginCountColors, totalColors);

            _targetColor = _colorConverter.TransformEnumToColor(targetColor);
            _beginRoundColors = ConvertColors(roundColors, 0, beginCount);
            _remainingColors = ConvertColors(roundColors, beginCount, totalColors - beginCount);
        }

        public void SetColorRecipes(ColorRecipes colorRecipes)
        {
            Guard.NotNull(colorRecipes, nameof(colorRecipes));

            _colorRecipes = colorRecipes;
        }

        private IReadOnlyList<EnumColor> BuildRoundColors(int count, int countMixeColor, out EnumColor target)
        {
            var result = new List<EnumColor>(count);
            var used = new HashSet<EnumColor>();

            target = PickRandomRecipeColor();
            (EnumColor first, EnumColor second) = _colorRecipes.GetPair(target);

            foreach (EnumColor color in new[] { first, second, target })
            {
                if (result.Count < count && used.Add(color))
                    result.Add(color);
            }

            while (result.Count < count)
            {
                IReadOnlyList<EnumColor> candidates = CollectMixableCandidates(used);

                EnumColor next = candidates.Count > 0
                    ? candidates[UnityEngine.Random.Range(0, candidates.Count)]
                    : PickRandomUnused(used);

                used.Add(next);
                result.Add(next);
            }

            return ReplaceWithMixtures(result, countMixeColor);
        }

        private IReadOnlyList<EnumColor> ReplaceWithMixtures(
            IReadOnlyList<EnumColor> colors,
            int countMixeColor)
        {
            var result = new List<EnumColor>(colors);
            var expanded = new HashSet<EnumColor>();

            for (int i = 0; i < countMixeColor; i++)
            {
                var candidates = new List<int>();

                for (int index = 0; index < result.Count; index++)
                {
                    EnumColor color = result[index];

                    if (expanded.Contains(color))
                        continue;

                    if (_colorRecipes.TryGetPair(color, out var pair) == false)
                        continue;

                    if (result.Contains(pair.First) || result.Contains(pair.Second))
                        continue;

                    candidates.Add(index);
                }

                if (candidates.Count == 0)
                    break;

                int replaceIndex = candidates[UnityEngine.Random.Range(0, candidates.Count)];
                EnumColor source = result[replaceIndex];
                (EnumColor first, EnumColor second) = _colorRecipes.GetPair(source);

                expanded.Add(source);
                expanded.Add(first);
                expanded.Add(second);
                result.RemoveAt(replaceIndex);
                result.Insert(replaceIndex, second);
                result.Insert(replaceIndex, first);
            }

            return result;
        }

        private IReadOnlyList<EnumColor> CollectMixableCandidates(HashSet<EnumColor> used)
        {
            List<EnumColor> selected = new List<EnumColor>(used);
            HashSet<EnumColor> found = new HashSet<EnumColor>();

            for (int i = 0; i < selected.Count; i++)
            {
                for (int j = i + 1; j < selected.Count; j++)
                {
                    if (_colorRecipes.TryGetResult(selected[i], selected[j], out EnumColor mixed)
                        && used.Contains(mixed) == false)
                    {
                        found.Add(mixed);
                    }
                }
            }

            return new List<EnumColor>(found);
        }

        private EnumColor PickRandomRecipeColor()
        {
            var withRecipe = new List<EnumColor>();

            foreach (EnumColor color in _allColors)
            {
                if (_colorRecipes.HasRecipe(color))
                    withRecipe.Add(color);
            }

            if (withRecipe.Count == 0)
                throw new InvalidOperationException("ColorRecipes has no recipes.");

            return withRecipe[UnityEngine.Random.Range(0, withRecipe.Count)];
        }

        private EnumColor PickRandomUnused(HashSet<EnumColor> used)
        {
            var unused = new List<EnumColor>();

            foreach (EnumColor color in _allColors)
            {
                if (used.Contains(color) == false)
                    unused.Add(color);
            }

            return unused[UnityEngine.Random.Range(0, unused.Count)];
        }

        private Color[] ConvertColors(
            IReadOnlyList<EnumColor> source,
            int startIndex,
            int length)
        {
            var colors = new Color[length];

            for (int i = 0; i < length; i++)
                colors[i] = _colorConverter.TransformEnumToColor(source[startIndex + i]);

            return colors;
        }
    }
}