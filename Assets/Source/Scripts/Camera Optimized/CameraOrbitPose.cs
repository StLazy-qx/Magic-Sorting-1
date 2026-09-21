using UnityEngine;

namespace Assets.Source.Scripts.Camera_Optimized
{
    public class CameraOrbitPose
    {
        private readonly Transform _target;
        private readonly Camera _camera;

        private float _radius;
        private float _azimuth;
        private float _initialAngle;
        private Vector3 _offset;
        private Quaternion _initialRotation;

        public CameraOrbitPose(Transform target, Camera camera)
        {
            _target = target;
            _camera = camera;
        }

        public void CaptureCurrentAsInitial()
        {
            Vector3 offset = _camera.transform.position - _target.position;
            _radius = offset.magnitude;

            if (_radius < Mathf.Epsilon)
            {
                _radius = 1f;
                offset = Vector3.back;
            }

            _offset = offset;
            _initialRotation = _camera.transform.rotation;
            _azimuth = Mathf.Atan2(offset.z, offset.x);
            Vector3 flat = new Vector3(offset.x, 0f, offset.z);
            _initialAngle = Mathf.Atan2(offset.y, flat.magnitude) * Mathf.Rad2Deg;
        }

        public void RestoreInitial()
        {
            _camera.transform.position = _target.position + _offset;
            _camera.transform.rotation = _initialRotation;
        }

        public void ApplyForNormalizedWidth(
            float normalizedWidth,
            float maxAngle,
            float minRadiusMultiplier,
            float maxRadiusMultiplier)
        {
            float angle = Mathf.Lerp(
                maxAngle, 
                _initialAngle, 
                normalizedWidth);
            float radiusMultiplier = Mathf.Lerp(
                maxRadiusMultiplier, 
                minRadiusMultiplier, 
                normalizedWidth);

            ApplyPose(angle, radiusMultiplier);
        }

        private void ApplyPose(float angleDeg, float radiusMultiplier)
        {
            Vector3 worldOffset = CalculateOffset(angleDeg, radiusMultiplier);
            _camera.transform.position = _target.position + worldOffset;
            _camera.transform.rotation = CalculateRotation(worldOffset);
        }

        private Vector3 CalculateOffset(float angleDeg, float radiusMultiplier)
        {
            float scaledRadius = _radius * radiusMultiplier;
            float rad = angleDeg * Mathf.Deg2Rad;
            float horizontal = Mathf.Cos(rad) * scaledRadius;
            float vertical = Mathf.Sin(rad) * scaledRadius;

            return new Vector3(
                Mathf.Cos(_azimuth) * horizontal,
                vertical,
                Mathf.Sin(_azimuth) * horizontal);
        }

        private Quaternion CalculateRotation(Vector3 worldOffset)
        {
            Vector3 lookDir = -worldOffset.normalized;
            Vector3 lookEuler = Quaternion.LookRotation(lookDir, Vector3.up).eulerAngles;
            lookEuler.y = _initialRotation.eulerAngles.y;

            return Quaternion.Euler(lookEuler);
        }
    }
}
