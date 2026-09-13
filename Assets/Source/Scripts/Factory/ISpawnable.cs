using UnityEngine;

namespace Assets.Source.Scripts.Factory
{
    public interface ISpawnable
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }
        public bool IsAlive { get; }

        public void Despawn();
    }
}
