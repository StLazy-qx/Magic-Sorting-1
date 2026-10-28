using System.Collections.Generic;
using Assets.Source.Scripts.Extensions;
using UnityEngine;

namespace Assets.Source.Scripts.Colorize
{
    public class ColorRandomizer : MonoBehaviour
    {
        private const int BeginCountColors = 2;
        private const int MinColorsCount = 4;

        private ColorCatalog _catalog;
        private ColorConverter _colorConverter;
        private ColorRecipes _colorRecipes;

        private IColorStrategy[] _allColors;
        private Color[] _beginRoundColors;
        private Color[] _remainingColors;
        private Color _targetColor;

        public IReadOnlyList<IColorStrategy> RoundColors => _allColors;
        public IReadOnlyList<Color> BeginColors => _beginRoundColors;
        public IReadOnlyList<Color> RemainingColors => _remainingColors;
        public Color TargetColor => _targetColor;

        public ColorCatalog Catalog => _catalog;
        public ColorConverter Converter => _colorConverter;
        public ColorRecipes Recipes => _colorRecipes;

        public void Initialize(ColorCatalog catalog, ColorConverter converter, ColorRecipes recipes)
        {
            Guard.NotNull(catalog, nameof(catalog));
            Guard.NotNull(converter, nameof(converter));
            Guard.NotNull(recipes, nameof(recipes));

            _catalog = catalog;
            _colorConverter = converter;
            _colorRecipes = recipes;
        }

        public void SetDifficultySettings(int colorsNumber)
        {
            Guard.Positive(colorsNumber, nameof(colorsNumber));

            EnsureInitialized();

            int count = NormalizeColorsCount(colorsNumber);
            _allColors = BuildRoundColors(count);

            Guard.NotNullOrEmpty(_allColors, nameof(_allColors));

            int beginCount = Mathf.Min(BeginCountColors, _allColors.Length);

            _targetColor = _colorConverter.TransformStrategyToColor(_allColors[0]);
            _beginRoundColors = ConvertColors(_allColors, 0, beginCount);
            _remainingColors = ConvertColors(_allColors, beginCount, _allColors.Length - beginCount);
        }

        private void EnsureInitialized()
        {
            Guard.IsTrue(
                _catalog != null && _colorConverter != null && _colorRecipes != null,
                nameof(ColorRandomizer),
                "ColorRandomizer is not initialized. " +
                "Call Initialize(catalog, converter, recipes) before use.");
        }

        private int NormalizeColorsCount(int requested)
        {
            int maxCount = _catalog.All.Count - _catalog.All.Count % 2;
            int count = Mathf.Clamp(requested, MinColorsCount, maxCount);
            return count - count % 2;
        }

        private IColorStrategy[] BuildRoundColors(int count)
        {
            List<IColorStrategy> result = new List<IColorStrategy>(count);
            HashSet<IColorStrategy> used = new HashSet<IColorStrategy>();

            IColorStrategy first = PickRandomRecipeColor();
            (IColorStrategy second, IColorStrategy third) = _colorRecipes.GetPair(first);

            AddColor(first, result, used);
            AddColor(second, result, used);
            AddColor(third, result, used);

            int safety = 0;
            while (result.Count < count)
            {
                Guard.IsTrue(++safety <= 1000,
                    $"BuildRoundColors exceeded 1000 iterations ({result.Count}/{count}).");

                AddColor(PickNextColor(result, used), result, used);
            }

            return result.ToArray();
        }

        private static void AddColor(IColorStrategy color, List<IColorStrategy> result, HashSet<IColorStrategy> used)
        {
            Guard.NotNull(color, nameof(color));

            if (used.Add(color))
                result.Add(color);
        }

        private IColorStrategy PickNextColor(List<IColorStrategy> selected, HashSet<IColorStrategy> used)
        {
            List<IColorStrategy> candidates = GetMixResults(selected, used);

            if (candidates.Count == 0)
                candidates = GetRecipeCompleters(used);

            if (candidates.Count == 0)
                candidates = GetUnusedColors(used);

            Guard.NotNullOrEmpty(candidates, nameof(candidates));

            return candidates[UnityEngine.Random.Range(0, candidates.Count)];
        }

        private List<IColorStrategy> GetMixResults(List<IColorStrategy> selected, HashSet<IColorStrategy> used)
        {
            List<IColorStrategy> results = new List<IColorStrategy>();

            for (int i = 0; i < selected.Count; i++)
            {
                for (int j = i + 1; j < selected.Count; j++)
                {
                    if (_colorRecipes.TryGetResult(selected[i], selected[j], out IColorStrategy result)
                        && used.Contains(result) == false
                        && results.Contains(result) == false)
                    {
                        results.Add(result);
                    }
                }
            }

            return results;
        }

        private List<IColorStrategy> GetUnusedColors(HashSet<IColorStrategy> used)
        {
            List<IColorStrategy> unused = new List<IColorStrategy>();

            foreach (IColorStrategy color in _catalog.All)
            {
                if (used.Contains(color) == false)
                    unused.Add(color);
            }

            return unused;
        }

        private IColorStrategy PickRandomRecipeColor()
        {
            List<IColorStrategy> withRecipe = new List<IColorStrategy>();

            foreach (IColorStrategy color in _catalog.All)
            {
                if (_colorRecipes.HasRecipe(color))
                    withRecipe.Add(color);
            }

            Guard.NotNullOrEmpty(withRecipe, nameof(withRecipe));

            return withRecipe[UnityEngine.Random.Range(0, withRecipe.Count)];
        }

        private List<IColorStrategy> GetRecipeCompleters(HashSet<IColorStrategy> used)
        {
            List<IColorStrategy> result = new List<IColorStrategy>();

            foreach (var recipe in _colorRecipes.ColorMixtures)
            {
                if (used.Contains(recipe.Key) == false)
                    continue;

                bool hasFirst = used.Contains(recipe.Value.First);
                bool hasSecond = used.Contains(recipe.Value.Second);

                if (hasFirst == hasSecond)
                    continue;

                IColorStrategy missing = hasFirst ? recipe.Value.Second : recipe.Value.First;

                if (result.Contains(missing) == false)
                    result.Add(missing);
            }

            return result;
        }

        private Color[] ConvertColors(IReadOnlyList<IColorStrategy> source, int startIndex, int length)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNegative(startIndex, nameof(startIndex));
            Guard.NotNegative(length, nameof(length));
            Guard.InRange(startIndex + length, 0, source.Count, nameof(length));

            Color[] colors = new Color[length];

            for (int i = 0; i < length; i++)
                colors[i] = _colorConverter.TransformStrategyToColor(source[startIndex + i]);

            return colors;
        }
    }
}