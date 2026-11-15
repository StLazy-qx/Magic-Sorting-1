using UnityEngine;

namespace Assets.Source.Scripts.Pool
{
    public class EffectPoolBinder : MonoBehaviour
    {
        [SerializeField] private ParticlePool _particlePool;
        [SerializeField] private MonoBehaviour _poolInitializable;

        private IEffectPoolInitializable _provider;

        private void Awake()
        {
            _provider = (IEffectPoolInitializable)_poolInitializable;
            _provider.PoolEffectSizeReading += OnInitPoolSize;
        }

        private void Start()
        {
            if (_provider.LastSize > 0)
                OnInitPoolSize(_provider.LastSize);
        }

        private void OnDestroy()
        {
            _provider.PoolEffectSizeReading -= OnInitPoolSize;
        }

        private void OnInitPoolSize(int totalColors)
        {
            _particlePool.Initialize(totalColors);
        }
    }
}
