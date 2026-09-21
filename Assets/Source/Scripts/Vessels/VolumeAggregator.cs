using UnityEngine;
using System;
using Assets.Source.Scripts.Extensions;

namespace Assets.Source.Scripts.Vessels
{
    public class VolumeAggregator : MonoBehaviour
    {
        [SerializeField] private Transform _internalVolume;
        [SerializeField] private LiquidAnimator _liquidAnimator;

        private int _vesselVolume;
        private int _currentSize = 0;
        private Liquid _liquid;

        public event Action<int> SizeChanged;

        public bool IsFull => _currentSize >= _vesselVolume;

        public void InitParameters(int vesselVolume, Liquid liquid)
        {
            Guard.Positive(vesselVolume, nameof(vesselVolume));
            Guard.NotNull(liquid, nameof(liquid));
            Guard.NotNull(_internalVolume, nameof(_internalVolume));
            Guard.NotNull(_liquidAnimator, nameof(_liquidAnimator));

            _vesselVolume = vesselVolume;
            _liquid = liquid;

            _liquidAnimator.Initialize(_internalVolume, _liquid, _vesselVolume);
        }

        public void GrowUpVolume()
        {
            _currentSize++;
            SizeChanged?.Invoke(_currentSize);
            _liquidAnimator.UpdateVolume(_currentSize);
        }
    }
}