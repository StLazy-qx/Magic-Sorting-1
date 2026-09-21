using Assets.Source.Scripts.Camera_Optimized;
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

    private CameraOrbitPose _pose;
    private bool _isInitialized;

    private void Awake()
    {
        if (_target == null || _mainCamera == null)
        {
            enabled = false;

            return;
        }

        _pose = new CameraOrbitPose(_target, _mainCamera);
        _pose.CaptureCurrentAsInitial();
        _isInitialized = true;
    }

    private void Start()
    {
        if (_isInitialized == false)
            return;

        Subscribe();
        ApplyCurrentWidthState();
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

    public void CaptureRoundStartPose()
    {
        if (_isInitialized == false)
            return;

        _pose.CaptureCurrentAsInitial();
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

        _pose.ApplyForNormalizedWidth(
            normalizedWidth, _maxAngle, _minRadiusMultiplier, _maxRadiusMultiplier);
    }

    private void OnThresholdExited()
    {
        if (_isInitialized == false)
            return;

        _pose.RestoreInitial();
    }
}