using System.Collections.Generic;
using Assets.Source.Scripts.Enums;

namespace Assets.Source.Scripts.Colorize
{
    public class ColorRecipes
    {
        private readonly Dictionary<EnumColor, (EnumColor First, EnumColor Second)> _colorMixtures =
            new Dictionary<EnumColor, (EnumColor First, EnumColor Second)>
        {
            { EnumColor.Red,      (EnumColor.Magenta, EnumColor.Yellow) },
            { EnumColor.Green,    (EnumColor.Cyan,    EnumColor.Yellow) },
            { EnumColor.Blue,     (EnumColor.Cyan,    EnumColor.Magenta) },
            { EnumColor.Yellow,   (EnumColor.Red,     EnumColor.Green) },
            { EnumColor.Orange,   (EnumColor.Red,     EnumColor.Yellow) },
            { EnumColor.Purple,   (EnumColor.Red,     EnumColor.Blue) },
            { EnumColor.Pink,     (EnumColor.Red,     EnumColor.White) },
            { EnumColor.Grey,     (EnumColor.Black,   EnumColor.White) },
            { EnumColor.Cyan,     (EnumColor.Green,   EnumColor.Blue) },
            { EnumColor.Lavender, (EnumColor.Purple,  EnumColor.White) },
        };

        private readonly Dictionary<(EnumColor, EnumColor), EnumColor> _resultsByPair =
            new Dictionary<(EnumColor, EnumColor), EnumColor>();

        private readonly HashSet<EnumColor> _ingredients = new HashSet<EnumColor>();

        public ColorRecipes()
        {
            foreach (var recipe in _colorMixtures)
            {
                _resultsByPair[Normalize(recipe.Value.First, recipe.Value.Second)] =
                    recipe.Key;

                _ingredients.Add(recipe.Value.First);
                _ingredients.Add(recipe.Value.Second);
            }
        }

        public IReadOnlyDictionary<EnumColor, (EnumColor First,
            EnumColor Second)> ColorMixtures => _colorMixtures;

        public bool HasRecipe(EnumColor color)
        {
            return _colorMixtures.ContainsKey(color);
        }

        public bool CanBeIngredient(EnumColor color)
        {
            return _ingredients.Contains(color);
        }

        public bool CanMix(EnumColor first, EnumColor second)
        {
            if (first == second)
                return false;

            return TryGetResult(first, second, out _);
        }

        public (EnumColor First, EnumColor Second) GetPair(EnumColor result)
        {
            if (_colorMixtures.TryGetValue(result, out var pair) == false)
                throw new KeyNotFoundException($"No recipe for color {result}.");

            return pair;
        }

        public bool TryGetPair(
            EnumColor result,
            out (EnumColor First, EnumColor Second) pair)
        {
            return _colorMixtures.TryGetValue(result, out pair);
        }

        public bool TryGetResult(
            EnumColor first,
            EnumColor second,
            out EnumColor result)
        {
            return _resultsByPair.TryGetValue(Normalize(first, second), out result);
        }

        private static (EnumColor, EnumColor) Normalize(EnumColor first, EnumColor second)
        {
            return first.CompareTo(second) <= 0 ? (first, second) : (second, first);
        }
    }
}