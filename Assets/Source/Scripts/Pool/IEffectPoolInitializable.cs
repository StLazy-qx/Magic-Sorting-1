using System;

namespace Assets.Source.Scripts.Pool
{
    public interface IEffectPoolInitializable
    {
        public event Action<int> PoolEffectSizeReading;
    }
}
