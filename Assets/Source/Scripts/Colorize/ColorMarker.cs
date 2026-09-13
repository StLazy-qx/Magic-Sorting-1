using Assets.Source.Scripts.Vessels;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.Colorize
{
    public class ColorMarker : MonoBehaviour
    {
        [SerializeField] private Vessel _vessel;
        [SerializeField] private Flag _flag;

        private Liquid _liquid;

        public void Initialize(Color color)
        {
            ValidateObjects();

            if (_vessel != null)
                _liquid = _vessel.Liquid;

            if (_vessel != null)
                _vessel.SetColor(color);

            if (_liquid != null)
                _liquid.SetColor(color);

            if (_flag != null)
                _flag.SetColor(color);
        }

        private void ValidateObjects()
        {
            if (_vessel == null)
            {
                throw new ArgumentNullException
                    (nameof(_vessel), "Vessel cannot be null");
            }

            if (_flag == null)
            {
                throw new ArgumentNullException
                    (nameof(_flag), "Flag cannot be null");
            }
        }
    }
}