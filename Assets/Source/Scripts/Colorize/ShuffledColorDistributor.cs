using Assets.Source.Scripts.Vessels;
using System.Collections.Generic;
using Assets.Source.Scripts.Pool;
using Assets.Source.Scripts.Extensions;
using System.Linq;
using UnityEngine;
using System;

namespace Assets.Source.Scripts.Colorize
{
    public class ShuffledColorDistributor : MonoBehaviour, IEffectPoolInitializable
    {
        private const int WaitingEffectsCount = 8;

        private IReadOnlyList<Vessel> _vessels;
        private List<Color> _colors = new List<Color>();
        private Queue<Color> _mixedColors = new Queue<Color>();

        public event Action<int> PoolEffectSizeReading;

        public void Initialize(IReadOnlyList<Vessel> vessels)
        {
            ValidateVessels(vessels);

            _vessels = vessels;

            GenerateColorList();
            ShuffleColors();
        }

        public bool TryGetRandomColor(out Color color)
        {
            return _mixedColors.TryDequeue(out color);
        }

        private void GenerateColorList()
        {
            _colors.Clear();

            foreach (Vessel vessel in _vessels)
            {
                if (vessel.Count <= 0)
                {
                    Guard.IsTrue(vessel.Count > 0,
                        $"Vessel count must be positive, but was {vessel.Count}");
                }

                for (int i = 0; i < vessel.Count; i++)
                {
                    _colors.Add(vessel.Color);
                }
            }

            PoolEffectSizeReading?.Invoke(
                _colors.Count + WaitingEffectsCount);
        }

        private void ShuffleColors()
        {
            int startRandomRange = 0;
            int stepIndex = 1;

            for (int i = _colors.Count - 1; i > 0; i--)
            {
                int randomNumber = UnityEngine.Random.
                    Range(startRandomRange, i + stepIndex);

                Color tempColor = _colors[i];
                _colors[i] = _colors[randomNumber];
                _colors[randomNumber] = tempColor;
            }

            _mixedColors.Clear();

            foreach (Color color in _colors)
                _mixedColors.Enqueue(color);
        }

        private void ValidateVessels(IReadOnlyList<Vessel> vessels)
        {
            Guard.NotNullOrEmpty(vessels, nameof(vessels));
            Guard.IsTrue(vessels.All(v => v != null),
                nameof(vessels),"The vessel list contains a zero element");
        }
    }
}