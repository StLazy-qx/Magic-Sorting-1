using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Colorize
{
    public class ColorCatalog
    {
        public IColorStrategy Red { get; } = new ColorStrategy("Red", new Color(1f, 0f, 0f));
        public IColorStrategy Green { get; } = new ColorStrategy("Green", new Color(0f, 1f, 0f));
        public IColorStrategy Blue { get; } = new ColorStrategy("Blue", new Color(0f, 0f, 1f));
        public IColorStrategy Yellow { get; } = new ColorStrategy("Yellow", new Color(1f, 1f, 0f));
        public IColorStrategy Orange { get; } = new ColorStrategy("Orange", new Color(1f, 0.5f, 0f));
        public IColorStrategy Purple { get; } = new ColorStrategy("Purple", new Color(0.5f, 0f, 0.5f));
        public IColorStrategy Pink { get; } = new ColorStrategy("Pink", new Color(1f, 0.15f, 0.55f));
        public IColorStrategy Grey { get; } = new ColorStrategy("Grey", new Color(0.6f, 0.6f, 0.6f));
        public IColorStrategy Cyan { get; } = new ColorStrategy("Cyan", new Color(0.2f, 1f, 1f));
        public IColorStrategy Lavender { get; } = new ColorStrategy("Lavender", new Color(0.7f, 0.5f, 0.9f));
        public IColorStrategy White { get; } = new ColorStrategy("White", new Color(1f, 1f, 1f));
        public IColorStrategy Black { get; } = new ColorStrategy("Black", new Color(0f, 0f, 0f));
        public IColorStrategy Magenta { get; } = new ColorStrategy("Magenta", new Color(1f, 0f, 1f));

        public IReadOnlyList<IColorStrategy> All { get; }

        public ColorCatalog()
        {
            All = new List<IColorStrategy>
            {
                Red, 
				Green, 
				Blue, 
				Yellow, 
				Orange, 
				Purple, 
				Pink, 
                Grey, 
				Cyan, 
				Lavender, 
				Black, 
				Magenta, 
				White
            };
        }
    }
}