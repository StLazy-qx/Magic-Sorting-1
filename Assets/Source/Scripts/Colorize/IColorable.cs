using UnityEngine;

namespace Assets.Source.Scripts.Colorize
{
    public interface IColorable
    {
        public Color Color { get; }

        public void SetColor(Color color);
    }
}