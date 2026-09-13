using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Tutorial
{
    public class AnimationParticle : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particle;

        private float _waitSecond = 2f;
        private Coroutine _playRoutine;
        private WaitForSeconds _waitForSeconds;

        private void Awake()
        {
            _waitForSeconds = new WaitForSeconds(_waitSecond);
        }

        public void Play(Vector3 position)
        {        
            if (_particle == null)
                return;

            transform.position = position;

            gameObject.SetActive(true);

            if (_playRoutine != null)
                StopCoroutine(_playRoutine);

            _playRoutine = StartCoroutine(PlayRoutine());
        }

        public void Play(Button button)
        {
            if (button == null)
                throw new ArgumentNullException(nameof(button));

            ParticleSystem particle = 
                button.GetComponentInChildren<ParticleSystem>(true);

            particle.Play();
        }

        private IEnumerator PlayRoutine()
        {
            _particle.Play();

            yield return _waitForSeconds;

            Stop();
        }

        public void Stop()
        {
            if (_particle == null)
                return;

            if (_particle.isPlaying)
                _particle.Stop();
        }
    }
}
