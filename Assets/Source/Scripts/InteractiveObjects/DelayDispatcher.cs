using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Scripts.InteractiveObjects
{
    class DelayDispatcher : MonoBehaviour
    {
        [SerializeField] private float _delay = 1.2f;

        private WaitForSeconds _wait;

        private void Awake() 
            => _wait = new WaitForSeconds(_delay);

        public void ExecuteAfterDelay(Action action)
        {
            if (action == null) 
                return;

            StartCoroutine(Routine(action));
        }

        private IEnumerator Routine(Action action)
        {
            yield return _wait;

            action?.Invoke();
        }
    }
}