using System;
using UnityEngine;

[DisallowMultipleComponent]
public class WindowWidthWatcher : MonoBehaviour
{
    [SerializeField] private int _activeWidth = 1200;
    [SerializeField] private int _minWidth = 200;
    
    private int _lastWidth = int.MinValue;
    private bool _isActive;

    public event Action<int> ThresholdReached;
    public event Action<int> WidthChanged;
    public event Action ThresholdExited;

    private void OnEnable()
    {
        _lastWidth = Screen.width;
        
        Evaluate(_lastWidth);
    }

    private void Update()
    {
        if (Screen.width != _lastWidth)
        {
            _lastWidth = Screen.width;
            
            Evaluate(_lastWidth);
        }
    }

    private void Evaluate(int width)
    {
        if (width <= _activeWidth)
        {
            if (_isActive == false)
            {
                _isActive = true;
                
                ThresholdReached?.Invoke(width);
            }

            int clamped = Mathf.Clamp(width, _minWidth, _activeWidth);
            
            WidthChanged?.Invoke(clamped);
        }
        else
        {
            if (_isActive)
            {
                _isActive = false;
                
                ThresholdExited?.Invoke();
            }
        }
    }
}