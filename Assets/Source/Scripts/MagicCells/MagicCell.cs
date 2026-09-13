using UnityEngine;
using Assets.Source.Scripts.Colorize;
using System;

namespace Assets.Source.Scripts.MagicCells
{
    [RequireComponent(typeof(Renderer))]

    public class MagicCell : MonoBehaviour, IColorable
    {
        [SerializeField] private ColorRandomizer _colorRandomizer;
        [SerializeField] private CellAnimator _animator;

        private Renderer _renderer;

        public event Action Interacted;

        public Color Color => _renderer.material.color;

        private void Awake()
        {
            if (_colorRandomizer == null)
                throw new ArgumentNullException(nameof(_colorRandomizer));

            _renderer = GetComponent<Renderer>();

            if (_renderer == null)
                throw new ArgumentNullException(nameof(_renderer));
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void SetColor(Color color)
        {
            _renderer.material.color = color;
        }

        private void OnMouseDown()
        {
            Interacted?.Invoke();
        }
    }
}