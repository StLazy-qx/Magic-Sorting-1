using UnityEngine;
using DG.Tweening;
using System;

namespace Assets.Source.Scripts.Vessels
{
    public class VolumeAggregator : MonoBehaviour
    {
        private const float Half = 0.5f;
        private const float AnimationDuration = 0.3f;

        [SerializeField] private Transform _internalVolume;

        private int _vesselVolume;
        private int _currentSize = 0;
        private float _deltaSize;
        private Vector3 _initialBottomPoint;
        private Liquid _liquid;

        public event Action<int> SizeChanged;

        public bool IsFull => _currentSize >= _vesselVolume;
        public int CurrentVolume => _currentSize;

        public void InitParameters(int vesselVolume, Liquid liquid)
        {
            _vesselVolume = vesselVolume > 0 ? vesselVolume :
                throw new ArgumentException("Объем сосуда должен быть больше 0", 
                nameof(vesselVolume));

            _liquid = liquid ??
                throw new ArgumentNullException(nameof(liquid), 
                "Жидкость не может быть null");

            if (_internalVolume == null)
            {
                throw new InvalidOperationException(
                    "Internal Volume не назначен в инспекторе");
            }

            _deltaSize = _internalVolume.localScale.y / _vesselVolume;

            SetupInitialLiquidPosition();
        }

        public void GrowUpVolume()
        {
            _currentSize++;
            SizeChanged?.Invoke(_currentSize);

            if (_liquid.gameObject.activeSelf == false)
                _liquid.gameObject.SetActive(true);

            UpdateLiquidVisual();
        }

        private void UpdateLiquidVisual()
        {
            float newHeight = _deltaSize * _currentSize;

            _liquid.transform
                .DOScaleY(newHeight, AnimationDuration)
                .SetEase(Ease.OutQuad);

            float yOffset = newHeight * Half;
            Vector3 newPosition = _initialBottomPoint + new Vector3(0, yOffset, 0);

            _liquid.transform
                .DOMoveY(newPosition.y, AnimationDuration)
                .SetEase(Ease.OutQuad);
        }

        private void SetupInitialLiquidPosition()
        {
            float halfHeight = _internalVolume.localScale.y * Half;
            _initialBottomPoint = _internalVolume.position - new Vector3(0, halfHeight, 0);

            _liquid.transform.localScale = new Vector3(
                _internalVolume.localScale.x,
                0f,
                _internalVolume.localScale.z
            );

            float yOffset = _deltaSize * Half;
            _liquid.transform.position = _initialBottomPoint + new Vector3(0, yOffset, 0);
        }
    }
}