using UnityEngine;

[DisallowMultipleComponent]
public class CameraHeightSetter : MonoBehaviour
{
    [SerializeField] private WindowWidthWatcher _widthWatcher;
    [SerializeField] private Transform _target;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _maxAngle = 76f;
    [SerializeField] private int _minWidth = 200;
    [SerializeField] private int _activeWidth = 1200;

    private float _initialAngle;
    private float _radius;
    private float _azimuth;
    private bool _isInitialized;

    private void Awake()
    {
        if (_target == null || _mainCamera == null)
        {
            enabled = false;
            
            return;
        }

        Vector3 offset = _mainCamera.transform.position - _target.position;
        _radius = offset.magnitude;

        if (_radius < Mathf.Epsilon)
        {
            _radius = 1f;
            offset = Vector3.back;
        }

        _azimuth = Mathf.Atan2(offset.z, offset.x);
        Vector3 flat = new Vector3(offset.x, 0f, offset.z);
        _initialAngle = Mathf.Atan2(offset.y, flat.magnitude) * Mathf.Rad2Deg;
        _isInitialized = true;
    }

    private void OnEnable()
    {
        if (_isInitialized == false) 
			return;

        if (_widthWatcher != null)
        {
            _widthWatcher.ThresholdReached += ApplyForWidth;
            _widthWatcher.WidthChanged += ApplyForWidth;
            _widthWatcher.ThresholdExited += OnThresholdExited;
        }

        ApplyForWidth(Screen.width);
    }

    private void OnDisable()
    {
        if (_widthWatcher != null)
        {
            _widthWatcher.ThresholdReached -= ApplyForWidth;
            _widthWatcher.WidthChanged -= ApplyForWidth;
            _widthWatcher.ThresholdExited -= OnThresholdExited;
        }
    }

    private void ApplyForWidth(int width)
    {
        if (_isInitialized == false) 
			return;

        float t = Mathf.InverseLerp(_minWidth, _activeWidth, width);
        float angle = Mathf.Lerp(_maxAngle, _initialAngle, t);

        SetElevation(angle);
    }

    private void OnThresholdExited()
    {
        if (_isInitialized == false) 
			return;
			
        SetElevation(_initialAngle);
    }

    private void SetElevation(float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        float horizontal = Mathf.Cos(rad) * _radius;
        float vertical   = Mathf.Sin(rad) * _radius;
        Vector3 worldOffset = new Vector3(
            Mathf.Cos(_azimuth) * horizontal,
            vertical,
            Mathf.Sin(_azimuth) * horizontal
        );
        _mainCamera.transform.position = _target.position + worldOffset;
        
        _mainCamera.transform.LookAt(_target.position);
    }
}