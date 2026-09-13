using Assets.Source.Scripts.Colorize;
using UnityEngine;
using System;

namespace Assets.Source.Scripts.Vessels
{
    [RequireComponent(typeof(Renderer))]

    public class Liquid : MonoBehaviour, IColorable
    {
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

            _renderer.material.color = color;

            gameObject.SetActive(false);
        }
    }
}