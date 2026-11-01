using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Pool;
using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Assets.Source.Scripts.ActionsHandlers
{
    public class ParticleMotionEffect : MonoBehaviour
    {
        [SerializeField] private ParticlePool _particlePool;
        [SerializeField] private float _moveDuration = 1f;

        private void Awake()
        {
            Guard.NotNull(_particlePool, nameof(_particlePool));
            Guard.IsTrue(_moveDuration > 0f, "Move duration must be positive.");
        }

        public void Play(Vector3 from, Vector3 to, Color color)
        {
            Guard.IsTrue(from.IsValid(), nameof(from), "Position contains NaN or Infinity");
            Guard.IsTrue(to.IsValid(), nameof(to), "Position contains NaN or Infinity");
            StartCoroutine(Run(from, to, color));
        }

        private IEnumerator Run(Vector3 from, Vector3 to, Color color)
        {
            ParticleSystem particle = _particlePool.HandOver();

            if (particle == null) 
                yield break;

            Setup(particle, from, color);

            float elapsed = 0f;

            while (elapsed < _moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _moveDuration);
                particle.transform.position = Vector3.Lerp(from, to, t);

                yield return null;
            }

            particle.Stop();
        }

        private static void Setup(ParticleSystem particle, Vector3 position, Color color)
        {
            particle.transform.position = position;
            particle.transform.rotation = Quaternion.identity;

            SetColor(particle, color);

            foreach (ParticleSystem child in 
                particle.GetComponentsInChildren<ParticleSystem>())
            {
                if (child != particle)
                    SetColor(child, color);
            }

            particle.Play();
        }

        private static void SetColor(ParticleSystem particle, Color color)
        {
            MainModule main = particle.main;
            main.startColor = color;
        }
    }
}