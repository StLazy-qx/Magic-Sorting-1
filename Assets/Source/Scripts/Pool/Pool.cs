using Assets.Source.Scripts.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Pool
{
    public abstract class Pool<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected readonly List<T> _objects = new();

        [SerializeField] protected Transform _container;

        public Transform Container => _container;
        public IReadOnlyList<T> Objects => _objects;

        public void ToList()
        {
            foreach (var item in _objects)
            {
                Debug.Log(item.gameObject.activeSelf);
            }
        }

        public void SetContainer(Transform container)
        {
            Guard.NotNull(container, nameof(container));

            _container = container;
        }

        public virtual void Add(T @object)
        {
            if (@object == null)
                throw new ArgumentNullException(nameof(@object));

            @object.transform.SetParent(_container);
            @object.gameObject.SetActive(false);

            if (_objects.Contains(@object) == false)
                _objects.Add(@object);
        }

        public virtual void ActivateAll()
        {
            foreach (T obj in _objects)
            {
                if (obj.gameObject.activeSelf == false)
                    obj.gameObject.SetActive(true);
            }
        }

        public virtual T Activate()
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                if (_objects[i].gameObject.activeSelf == false)
                {
                    T @object = _objects[i];
                    @object.gameObject.SetActive(true);

                    OnActivated(@object);
                    return @object;
                }
            }

            return null;
        }

        public virtual void Deactivate(T @object)
        {
            if (@object == null)
                throw new ArgumentNullException(nameof(@object));

            if (_objects.Contains(@object) == false)
                return;

            @object.gameObject.SetActive(false);

            OnDeactivated(@object);
        }

        public IReadOnlyList<T> GetActiveObjects()
        {
            List<T> activeObjects = new();

            for (int i = 0; i < _objects.Count; i++)
            {
                if (_objects[i].gameObject.activeSelf)
                    activeObjects.Add(_objects[i]);
            }

            return activeObjects;
        }

        public virtual void Clear()
        {
            _objects.Clear();
        }

        protected virtual void OnActivated(T obj) { }

        protected virtual void OnDeactivated(T obj) { }
    }
}
