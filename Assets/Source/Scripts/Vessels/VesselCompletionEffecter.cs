using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Pool;
using System.Collections;
using UnityEngine;
using System;

namespace Assets.Source.Scripts.Vessels
{
    public class VesselCompletionEffecter : MonoBehaviour, IEffectPoolInitializable
    {
        private readonly float _offsetY = 0.3f;

        [SerializeField] private ParticlePool _particlePool;

        public event Action<int> PoolEffectSizeReading;

        public int LastSize { get; private set; }

        public void Initialize(int value)
        {
            Guard.Positive(value, nameof(value));

            LastSize = value;

            PoolEffectSizeReading?.Invoke(value);
        }

        public void ResetPool()
        {
            Guard.NotNull(_particlePool, nameof(_particlePool));

            _particlePool.ReturnAll();
        }

        public IEnumerator PlayEffect(
            Vector3 position,
            Color color)
        {
            Guard.NotNull(_particlePool, nameof(_particlePool));

            float timeEndSession = 2f;
            ParticleSystem particle = _particlePool.HandOver();

            while (particle == null)
            {
                yield return null;
                particle = _particlePool.HandOver();
            }

            particle.transform.position = new Vector3
                (position.x, position.y - _offsetY, position.z);

            SetParticleStartColor(particle, color);
            SetChildParticlesStartColor(particle, color);
            particle.Play();

            yield return new WaitForSeconds(timeEndSession);

            //Guard.NotNull(_particlePool, nameof(_particlePool));

            //float timeEndSession = 2f;
            //ParticleSystem particle = _particlePool.HandOver();

            //Guard.IsTrue(particle != null, "No available particles in pool.");

            //particle.transform.position = new Vector3
            //    (position.x, position.y - _offsetY, position.z);

            //SetParticleStartColor(particle, color);
            //SetChildParticlesStartColor(particle, color);
            //particle.Play();

            //yield return new WaitForSeconds(timeEndSession);
        }

        private void SetParticleStartColor(ParticleSystem particleSystem, Color color)
        {
            Guard.NotNull(particleSystem, nameof(particleSystem));

            ParticleSystem.MainModule main = particleSystem.main;
            main.startColor = color;
        }

        private void SetChildParticlesStartColor(ParticleSystem parent, Color color)
        {
            Guard.NotNull(parent, nameof(parent));

            ParticleSystem[] children = parent.GetComponentsInChildren<ParticleSystem>();

            foreach (var child in children)
            {
                if (child != parent)
                    SetParticleStartColor(child, color);
            }

            ParticleSystem.SubEmittersModule subEmitters = parent.subEmitters;

            for (int i = 0; i < subEmitters.subEmittersCount; i++)
            {
                ParticleSystem subEmitter = subEmitters.GetSubEmitterSystem(i);

                if (subEmitter != null)
                    SetParticleStartColor(subEmitter, color);
            }
        }
    }
}