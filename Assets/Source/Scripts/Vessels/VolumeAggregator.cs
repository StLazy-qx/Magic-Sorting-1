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
        public int CurrentVolume => _currentSize;

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

        // >>> FIX 2: НОВЫЙ метод. Раньше _currentSize не сбрасывался нигде,
        // поэтому переиспользованный сосуд приходил в новый раунд с прошлым объёмом.
        // Если у LiquidAnimator есть метод мгновенного сброса без анимации,
        // лучше вызвать его вместо UpdateVolume(0) (я этот класс не видел).
        public void ResetVolume()
        {
            _currentSize = 0;
            SizeChanged?.Invoke(_currentSize);

            if (_liquidAnimator != null)
                _liquidAnimator.UpdateVolume(0);
        }
    }
}

//namespace Assets.Source.Scripts.Vessels
//{
//    public class VolumeAggregator : MonoBehaviour
//    {
//        [SerializeField] private Transform _internalVolume;
//        [SerializeField] private LiquidAnimator _liquidAnimator;

//        private int _vesselVolume;
//        private int _currentSize = 0;
//        private Liquid _liquid;

//        public event Action<int> SizeChanged;

//        public bool IsFull => _currentSize >= _vesselVolume;
//        public int CurrentVolume => _currentSize;

//        public void InitParameters(int vesselVolume, Liquid liquid)
//        {
//            Guard.Positive(vesselVolume, nameof(vesselVolume));
//            Guard.NotNull(liquid, nameof(liquid));
//            Guard.NotNull(_internalVolume, nameof(_internalVolume));
//            Guard.NotNull(_liquidAnimator, nameof(_liquidAnimator));

//            _vesselVolume = vesselVolume;
//            _liquid = liquid;

//            _liquidAnimator.Initialize(_internalVolume, _liquid, _vesselVolume);
//        }

//        public void GrowUpVolume()
//        {
//            _currentSize++;
//            SizeChanged?.Invoke(_currentSize);
//            _liquidAnimator.UpdateVolume(_currentSize);
//        }
//    }
//}