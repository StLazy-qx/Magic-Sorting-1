using Assets.Source.Scripts.MagicCells;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Scripts.Tutorial
{
    public class DelayedActionTimer : MonoBehaviour
    {
        private Coroutine _activeTimer;

        public void StartTimer(float delayInSeconds, Action onElapsed)
        {
            StopTimer();

            _activeTimer = StartCoroutine(
                RunTimer(delayInSeconds, onElapsed));
        }

        public void StopTimer()
        {
            if (_activeTimer != null)
            {
                StopCoroutine(_activeTimer);

                _activeTimer = null;
            }
        }

        private IEnumerator RunTimer(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();

            _activeTimer = null;
        }

        //[SerializeField] private MagicCellRouter _cellRouter;
        //[SerializeField] private float _initialDelay = 1f;
        //[SerializeField] private float _repeatDelay = 7f;

        //private Coroutine _currentRoutine;

        //public event Action SearchRequested;

        //private void OnEnable()
        //{
        //    _cellRouter.CellDeparturing += OnCellDeparturing;
        //}

        //private void OnDisable()
        //{
        //    _cellRouter.CellDeparturing -= OnCellDeparturing;
        //    StopAllCoroutines();
        //    _currentRoutine = null;
        //}

        //private void Start()
        //{
        //    StartTimer(_initialDelay);
        //}

        //private void OnCellDeparturing()
        //{
        //    StartTimer(_repeatDelay);
        //}

        //private void StartTimer(float delay)
        //{
        //    if (_currentRoutine != null)
        //        StopCoroutine(_currentRoutine);

        //    _currentRoutine = StartCoroutine(TimerRoutine(delay));
        //}

        //private IEnumerator TimerRoutine(float delay)
        //{
        //    yield return new WaitForSeconds(delay);

        //    SearchRequested?.Invoke();
        //    _currentRoutine = null;
        //}
    }
}
