using Assets.Source.Scripts.Camera_Optimized;
using Assets.Source.Scripts.Extensions;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraHeightSetter : MonoBehaviour
{
    [SerializeField] private WindowWidthWatcher _widthWatcher;
    [SerializeField] private Transform _target;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _maxAngle = 72f;
    [Header("Radius Scale")]
    [SerializeField] private float _minRadiusMultiplier = 1f;
    [SerializeField] private float _maxRadiusMultiplier = 2f;

    private bool _isInitialized;
    private CameraOrbitPose _pose;
    
    private void Awake()
    {
        Guard.NotNull(_target, nameof(_target));
        Guard.NotNull(_mainCamera, nameof(_mainCamera));
        ValidateSettings();

        if (_target == null || _mainCamera == null)
        {
            enabled = false;

            return;
        }

        _pose = new CameraOrbitPose(_target, _mainCamera);

        _pose.CaptureCurrentAsInitial();

        _isInitialized = true;
    }

    private void OnEnable()
    {
        if (_isInitialized == false)
            return;

        Subscribe();
        ApplyCurrentWidthState();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (_widthWatcher == null)
            return;

        _widthWatcher.ThresholdReached += OnWidthChanged;
        _widthWatcher.WidthChanged += OnWidthChanged;
        _widthWatcher.ThresholdExited += OnThresholdExited;
    }

    private void Unsubscribe()
    {
        if (_widthWatcher == null)
            return;

        _widthWatcher.ThresholdReached -= OnWidthChanged;
        _widthWatcher.WidthChanged -= OnWidthChanged;
        _widthWatcher.ThresholdExited -= OnThresholdExited;
    }

    private void ApplyCurrentWidthState()
    {
        if (_widthWatcher == null || _widthWatcher.IsActive == false)
        {
            _pose.RestoreInitial();

            return;
        }

        OnWidthChanged(_widthWatcher.NormalizedWidth);
    }

    private void OnWidthChanged(float normalizedWidth)
    {
        if (_isInitialized == false)
            return;

        Guard.InRange(
            normalizedWidth, 
            0f, 
            1f, 
            nameof(normalizedWidth));

        _pose.ApplyForNormalizedWidth(
            normalizedWidth, _maxAngle, _minRadiusMultiplier, _maxRadiusMultiplier);
    }

    private void OnThresholdExited()
    {
        if (_isInitialized == false)
            return;

        _pose.RestoreInitial();
    }

    private void ValidateSettings()
    {
        Guard.InRange(_maxAngle, 0f, 90f, nameof(_maxAngle));
        Guard.IsTrue(_minRadiusMultiplier > 0f, nameof(_minRadiusMultiplier), "Значение должно быть больше 0.");
        Guard.IsTrue(_maxRadiusMultiplier > 0f, nameof(_maxRadiusMultiplier), "Значение должно быть больше 0.");
        Guard.IsTrue(_minRadiusMultiplier <= _maxRadiusMultiplier, nameof(_minRadiusMultiplier),
            "Значение не должно превышать maxRadiusMultiplier.");
    }
}