using Assets.Source.Scripts.Colorize;
using UnityEngine;
using System;

namespace Assets.Source.Scripts.Vessels
{
    [RequireComponent(typeof(Renderer))]

    public class Flag : MonoBehaviour, IColorable
    {
        private const float FabricDarkenFactor = 0.75f;
        private const float Saturation = 0.8f;

        private Renderer _renderer;

        public Color Color => _renderer.material.color;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();

            if (_renderer == null)
            {
                throw new InvalidOperationException(
                    "Renderer component is missing");
            }
        }

        public void SetColor(Color color)
        {
            if (_renderer.material == null)
            {
                throw new InvalidOperationException(
                    "Renderer material is not assigned");
            }

            _renderer.material.color = AdjustedColor(color);
        }

        private Color AdjustedColor(Color color)
        {
            Color.RGBToHSV(color, out float hue, out float saturation, out float value);

            saturation *= Saturation;
            value *= FabricDarkenFactor;

            return Color.HSVToRGB(hue, Mathf.Clamp01(saturation), Mathf.Clamp01(value));
        }
    }
}