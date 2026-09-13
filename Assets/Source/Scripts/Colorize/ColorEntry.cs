using Assets.Source.Scripts.Extensions;
using UnityEngine;

namespace Assets.Source.Scripts.Colorize
{
    public class ColorEntry
    {
        private readonly Color _mainColor;
        private int _beginCountColor;

        public ColorEntry(Color mainColor, int count)
        {
            Guard.Positive(count, nameof(count));

            _mainColor = mainColor;
            _beginCountColor = count;
        }

        public Color ConsumeColor()
        {
            if (_beginCountColor <= 0)
                return default;

            _beginCountColor--;

            return _mainColor;
        }

        public bool IsEmpty() 
            => _beginCountColor == 0;
    }
}
