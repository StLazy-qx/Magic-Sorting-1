using System.Collections.Generic;

namespace Assets.Source.Scripts.Colorize
{
    public class ColorRecipes
    {
        private readonly Dictionary<IColorStrategy, (IColorStrategy First, IColorStrategy Second)> _colorMixtures;
        private readonly Dictionary<(IColorStrategy, IColorStrategy), IColorStrategy> _resultsByPair = new();
        private readonly HashSet<IColorStrategy> _ingredients = new();

        public ColorRecipes(ColorCatalog catalog)
        {
            _colorMixtures = new Dictionary<IColorStrategy, (IColorStrategy First, IColorStrategy Second)>
            {
                { catalog.Red,      (catalog.Magenta, catalog.Yellow) },
                { catalog.Green,    (catalog.Cyan,    catalog.Yellow) },
                { catalog.Blue,     (catalog.Cyan,    catalog.Magenta) },
                { catalog.Yellow,   (catalog.Red,     catalog.Green) },
                { catalog.Orange,   (catalog.Red,     catalog.Yellow) },
                { catalog.Purple,   (catalog.Red,     catalog.Blue) },
                { catalog.Pink,     (catalog.Red,     catalog.White) },
                { catalog.Grey,     (catalog.Black,   catalog.White) },
                { catalog.Cyan,     (catalog.Green,   catalog.Blue) },
                { catalog.Lavender, (catalog.Purple,  catalog.White) },
            };

            foreach (var recipe in _colorMixtures)
            {
                _resultsByPair[Normalize(recipe.Value.First, recipe.Value.Second)] = recipe.Key;
                _ingredients.Add(recipe.Value.First);
                _ingredients.Add(recipe.Value.Second);
            }
        }

        public IReadOnlyDictionary<IColorStrategy, (IColorStrategy First, IColorStrategy Second)> ColorMixtures => _colorMixtures;

        public bool HasRecipe(IColorStrategy color) => _colorMixtures.ContainsKey(color);
        public bool CanBeIngredient(IColorStrategy color) => _ingredients.Contains(color);

        public bool CanMix(IColorStrategy first, IColorStrategy second)
        {
            if (first == second) return false;
            return TryGetResult(first, second, out _);
        }

        public (IColorStrategy First, IColorStrategy Second) GetPair(IColorStrategy result)
        {
            if (_colorMixtures.TryGetValue(result, out var pair) == false)
                throw new KeyNotFoundException($"No recipe for color {result.Name}.");

            return pair;
        }

        public bool TryGetPair(IColorStrategy result, out (IColorStrategy First, IColorStrategy Second) pair)
        {
            return _colorMixtures.TryGetValue(result, out pair);
        }

        public bool TryGetResult(IColorStrategy first, IColorStrategy second, out IColorStrategy result)
        {
            return _resultsByPair.TryGetValue(Normalize(first, second), out result);
        }

        private static (IColorStrategy, IColorStrategy) Normalize(IColorStrategy first, IColorStrategy second)
        {
            return string.Compare(first.Name, second.Name, System.StringComparison.Ordinal) <= 0 
                ? (first, second) 
                : (second, first);
        }
    }
}