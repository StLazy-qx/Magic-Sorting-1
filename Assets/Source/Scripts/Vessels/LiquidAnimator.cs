using UnityEngine;
using DG.Tweening;
using Assets.Source.Scripts.Extensions;

namespace Assets.Source.Scripts.Vessels
{
    class LiquidAnimator : MonoBehaviour
    {
        private const float AnimationDuration = 0.3f;

        private Liquid _liquid;
        private Transform _liquidTransform;
        private LiquidPlacementCalculator _calculator;

        public void Initialize(Transform internalVolume, 
            Liquid liquid, int vesselVolume)
        {
            Guard.NotNull(liquid, nameof(liquid));
            Guard.Positive(vesselVolume, nameof(vesselVolume));

            _liquid = liquid;
            _liquidTransform = liquid.transform;
            _calculator = new LiquidPlacementCalculator(internalVolume, vesselVolume);

            SetupInitialState();
        }

        public void UpdateVolume(int currentVolume)
        {
            if (_liquid == null || _calculator == null)
                return;

            if (currentVolume > 0 && !_liquid.gameObject.activeSelf)
                _liquid.gameObject.SetActive(true);

            float targetHeight = _calculator.GetHeight(currentVolume);
            float targetY = _calculator.GetTargetY(currentVolume);

            _liquidTransform.DOKill();
            _liquidTransform
                .DOScaleY(targetHeight, AnimationDuration)
                .SetEase(Ease.OutQuad);
            _liquidTransform
                .DOMoveY(targetY, AnimationDuration)
                .SetEase(Ease.OutQuad);
        }

        private void SetupInitialState()
        {
            _liquidTransform.localScale = _calculator.GetInitialScale();
            _liquidTransform.position = _calculator.GetInitialPosition();
        }
    }
}
