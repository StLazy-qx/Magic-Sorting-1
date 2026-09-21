using System;
using UnityEngine;

[DisallowMultipleComponent]
public class WindowWidthWatcher : MonoBehaviour
{
    [SerializeField] private int _activeWidth = 900;
    [SerializeField] private int _minWidth = 150;
    
    private int _lastWidth = int.MinValue;
    private bool _isActive;
    private float _normalizedWidth = 1f;

    public event Action<float> ThresholdReached;
    public event Action<float> WidthChanged;
    public event Action ThresholdExited;

    public bool IsActive => _isActive;
    public float NormalizedWidth => _normalizedWidth;

    private void OnEnable()
    {
        _lastWidth = Screen.width;

        Evaluate(_lastWidth);
    }

    private void Update()
    {
        if (Screen.width == _lastWidth)
            return;

        _lastWidth = Screen.width;

        Evaluate(_lastWidth);
    }

    private void Evaluate(int width)
    {
        if (width <= _activeWidth)
        {
            _normalizedWidth = Mathf.InverseLerp(_minWidth, _activeWidth, width);

            if (_isActive == false)
            {
                _isActive = true;

                ThresholdReached?.Invoke(_normalizedWidth);
            }

            WidthChanged?.Invoke(_normalizedWidth);
        }
        else
        {
            if (_isActive == false)
                return;

            _isActive = false;
            _normalizedWidth = 1f;

            ThresholdExited?.Invoke();
        }
    }
}