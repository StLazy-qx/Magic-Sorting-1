using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Colorize
{
    public class ColorConverter
    {
        private const float ExactMatchEpsilon = 0.0001f;

        private readonly ColorCatalog _catalog;
        private readonly Dictionary<IColorStrategy, Color> _strategyToColor;
        private readonly List<IColorStrategy> _all;

        public ColorConverter(ColorCatalog catalog)
        {
            _catalog = catalog;

            _strategyToColor = new Dictionary<IColorStrategy, Color>(catalog.All.Count);
            _all = new List<IColorStrategy>(catalog.All);

            foreach (var s in catalog.All)
                _strategyToColor[s] = s.Value;
        }

        public Color TransformStrategyToColor(IColorStrategy strategy)
        {
            if (strategy == null)
                throw new System.ArgumentNullException(nameof(strategy));

            return _strategyToColor.TryGetValue(strategy, out var c) ? c : strategy.Value;
        }

        public IColorStrategy TransformColorToStrategy(Color color)
        {
            foreach (var s in _all)
            {
                Color c = s.Value;
                if (Mathf.Abs(c.r - color.r) < ExactMatchEpsilon
                 && Mathf.Abs(c.g - color.g) < ExactMatchEpsilon
                 && Mathf.Abs(c.b - color.b) < ExactMatchEpsilon)
                    return s;
            }

            IColorStrategy closest = null;
            float minDist = float.MaxValue;

            foreach (var s in _all)
            {
                Color c = s.Value;
                float dr = c.r - color.r;
                float dg = c.g - color.g;
                float db = c.b - color.b;
                float d = dr * dr + dg * dg + db * db;

                if (d < minDist) { minDist = d; closest = s; }
            }

            return closest;
        }
    }
}