using Assets.Source.Scripts.Extensions;
using UnityEngine;

namespace Assets.Source.Scripts.Camera_Optimized
{
    public class CameraOrbitPose
    {
        private const float MinRadiusSqr = 1e-8f;
        private const float MinHorizontal = 1e-5f;

        private readonly Transform _target;
        private readonly Transform _cameraTransform;

        private Vector3 _initialOffset;
        private Quaternion _initialRotation;
        private float _radius;
        private float _initialAngleDeg;
        private float _cosAzimuth;
        private float _sinAzimuth;
        private float _initialYaw;

        public CameraOrbitPose(Transform target, Camera camera)
        {
            Guard.NotNull(target, nameof(target));
            Guard.NotNull(camera, nameof(camera));

            _target = target;
            _cameraTransform = camera.transform;
        }

        public void CaptureCurrentAsInitial()
        {
            Vector3 offset = _cameraTransform.position - _target.position;
            float sqrRadius = offset.sqrMagnitude;

            if (sqrRadius < MinRadiusSqr)
            {
                offset = Vector3.back;
                sqrRadius = 1f;
            }

            float horizontal = Mathf.Sqrt
                (offset.x * offset.x + offset.z * offset.z);
            _radius = Mathf.Sqrt(sqrRadius);
            _initialOffset = offset;
            _initialRotation = _cameraTransform.rotation;
            _initialYaw = _initialRotation.eulerAngles.y;
            _initialAngleDeg = Mathf.Atan2(offset.y, horizontal) * Mathf.Rad2Deg;

            if (horizontal > MinHorizontal)
            {
                float invHorizontal = 1f / horizontal;
                _cosAzimuth = offset.x * invHorizontal;
                _sinAzimuth = offset.z * invHorizontal;
            }
            else
            {
                _cosAzimuth = 1f;
                _sinAzimuth = 0f;
            }
        }

        public void RestoreInitial()
        {
            _cameraTransform.SetPositionAndRotation(
                _target.position + _initialOffset,
                _initialRotation
            );
        }

        public void ApplyForNormalizedWidth(
            float normalizedWidth,
            float maxAngle,
            float minRadiusMultiplier,
            float maxRadiusMultiplier)
        {
            Guard.InRange(normalizedWidth, 0f, 1f, nameof(normalizedWidth));
            Guard.InRange(maxAngle, 0f, 90f, nameof(maxAngle));
            Guard.IsTrue(minRadiusMultiplier > 0f, nameof(minRadiusMultiplier), "Значение должно быть больше 0.");
            Guard.IsTrue(maxRadiusMultiplier > 0f, nameof(maxRadiusMultiplier), "Значение должно быть больше 0.");
            Guard.IsTrue(minRadiusMultiplier <= maxRadiusMultiplier, nameof(minRadiusMultiplier),
                "Значение не должно превышать maxRadiusMultiplier.");

            float t = Mathf.Clamp01(normalizedWidth);
            float angleDeg = maxAngle + (_initialAngleDeg - maxAngle) * t;
            float radiusMultiplier =
                maxRadiusMultiplier + (minRadiusMultiplier - maxRadiusMultiplier) * t;

            ApplyPose(angleDeg, radiusMultiplier);
        }

        private void ApplyPose(float angleDeg, float radiusMultiplier)
        {
            float scaledRadius = _radius * radiusMultiplier;
            float angleRad = angleDeg * Mathf.Deg2Rad;
            float sinAngle = Mathf.Sin(angleRad);
            float cosAngle = Mathf.Cos(angleRad);
            float horizontalDistance = cosAngle * scaledRadius;
            float verticalDistance = sinAngle * scaledRadius;
            Vector3 worldOffset = new Vector3(
                _cosAzimuth * horizontalDistance,
                verticalDistance,
                _sinAzimuth * horizontalDistance
            );
            Quaternion rotation = Quaternion.Euler(angleDeg, _initialYaw, 0f);

            _cameraTransform.SetPositionAndRotation(
                _target.position + worldOffset,
                rotation
            );
        }
    }
}
