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

        public IReadOnlyDictionary<EnumColor, 
			(EnumColor First, EnumColor Second)> 
				ColorMixtures => _colorMixtures;

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
            foreach (KeyValuePair<EnumColor, 
				(EnumColor First, EnumColor Second)> 
					keyValuePair in _colorMixtures)
            {
                (EnumColor firstRecipeColor, 
					EnumColor secondRecipeColor) = keyValuePair.Value;

                bool isDirectOrder = 
					firstRecipeColor == first && 
					secondRecipeColor == second;
                bool isReversedOrder = 
					firstRecipeColor == second && 
					secondRecipeColor == first;

                if (isDirectOrder || isReversedOrder)
                {
                    result = keyValuePair.Key;
                    
                    return true;
                }
            }

            result = default;
            
            return false;
        }
    }
}