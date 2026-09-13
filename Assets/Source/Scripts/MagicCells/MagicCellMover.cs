using DG.Tweening;
using UnityEngine;

namespace Assets.Source.Scripts.MagicCells
{
    public class MagicCellMover : MonoBehaviour
    {
        private const float Half = 0.5f;

        [SerializeField] private float _arcMoveDuration = 1f;
        [SerializeField] private float _sideOffsetX = 1f;

        private Tween _moveTween;

        private void Awake()
        {
            ValidateObjects();
        }

        private void OnDestroy()
        {
            _moveTween?.Kill();
        }

        public void MoveArc(Vector3 targetPosition)
        {
            _moveTween?.Kill();

            Vector3 start = transform.position;
            Vector3 mid = (start + targetPosition) * Half;
            mid.x += _sideOffsetX;

            Vector3[] path =
            {
                start,
                mid,
                targetPosition
            };

            _moveTween = transform
                .DOPath(path, _arcMoveDuration, PathType.CatmullRom)
                .SetEase(Ease.Linear)
                .SetLink(gameObject)
                .OnComplete(() =>
                {
                    transform.position = targetPosition;
                    _moveTween = null;
                });
        }

        private void ValidateObjects()
        {
            if (_arcMoveDuration <= 0)
            {
                throw new System.ArgumentException(
                    "Move distance must be positive", nameof(_arcMoveDuration));
            }

            if (_sideOffsetX <= 0)
            {
                throw new System.ArgumentException(
                    "Move duration must be positive", nameof(_sideOffsetX));
            }
        }
    }
}