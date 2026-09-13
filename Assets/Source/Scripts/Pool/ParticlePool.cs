using Assets.Source.Scripts.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Pool
{
    // наследование от пула
    //разделить на фабрику и пул
    public class ParticlePool : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particlePrefab;
        [SerializeField] private Transform _container;

        private List<ParticleSystem> _particles = new();

        private void Awake()
        {
            _container.SetParent(transform);
        }

        public void Initialize(int count)
        {
            ValidateArguments(count);

            int difference = count - _particles.Count;

            if (difference > 0)
                CreateParticles(difference);
            else if (difference < 0)
                DestroyParticles(-difference);

            Reset();
        }

        public ParticleSystem HandOver()
        {
            foreach (ParticleSystem particle in _particles)
            {
                if (particle.gameObject.activeSelf == false)
                {
                    particle.gameObject.SetActive(true);

                    return particle;
                }
            }

            return null;
        }

        private void CreateParticles(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                ParticleSystem particle = 
                    Instantiate(_particlePrefab, _container);

                particle.gameObject.SetActive(false);
                _particles.Add(particle);
            }
        }

        private void DestroyParticles(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                int lastIndex = _particles.Count - 1;
                ParticleSystem particle = _particles[lastIndex];

                _particles.RemoveAt(lastIndex);

                if (particle != null)
                    Destroy(particle.gameObject);
            }
        }

        private void Reset()
        {
            foreach (ParticleSystem particle in _particles)
            {
                if (particle != null)
                    particle.gameObject.SetActive(false);
            }
        }

        private void ValidateArguments(int count)
        {
            Guard.NotNull(_particlePrefab, nameof(_particlePrefab));
            Guard.NotNull(_container, nameof(_container));
            Guard.Positive(count, nameof(count));
        }
    }
}