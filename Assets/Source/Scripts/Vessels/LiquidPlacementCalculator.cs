using UnityEngine;
using Assets.Source.Scripts.Extensions;

namespace Assets.Source.Scripts.Vessels
{
    class LiquidPlacementCalculator
    {
        private const float Half = 0.5f;
        private readonly Transform _internalVolume;
        private readonly float _deltaSize;
        private readonly float _halfDeltaSize;
        private readonly float _initialBottomY;
        private readonly float _initialX;
        private readonly float _initialZ;
        private readonly Vector3 _initialScale;

        public LiquidPlacementCalculator(Transform internalVolume, int vesselVolume)
        {
            Guard.NotNull(internalVolume, nameof(internalVolume));
            Guard.Positive(vesselVolume, nameof(vesselVolume));

            _internalVolume = internalVolume;
            float internalHeight = internalVolume.localScale.y;
            _deltaSize = internalHeight / vesselVolume;
            _halfDeltaSize = _deltaSize * Half;

            Vector3 volumePos = internalVolume.position;
            _initialBottomY = volumePos.y - internalHeight * Half;
            _initialX = volumePos.x;
            _initialZ = volumePos.z;

            _initialScale = new Vector3(
                internalVolume.localScale.x,
                0f,
                internalVolume.localScale.z
            );
        }

        public float GetHeight(int currentVolume)
            => _deltaSize * currentVolume;

        public float GetTargetY(int currentVolume)
            => _initialBottomY + GetHeight(currentVolume) * Half;

        public Vector3 GetInitialScale()
            => _initialScale;

        public Vector3 GetInitialPosition()
            => new Vector3(_initialX, _initialBottomY 
                + _halfDeltaSize, _initialZ);
    }
}
