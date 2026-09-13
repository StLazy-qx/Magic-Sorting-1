using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.Factory
{
    public abstract class Factory<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected T Prefab;
        [SerializeField] protected Transform[] SpawnPoints;
        [SerializeField] protected DifficultyDatabase DifficultyDatabase;

        protected DifficultyState DifficultyState;
        protected DifficultySettings CurrentSettings;
        private List<T> _objects = new List<T>();

        public event Action<IReadOnlyList<T>> ListObjectsChanged;

        public IReadOnlyList<T> Objects => _objects;

        protected virtual void OnDestroy()
        {
            if (DifficultyState != null)
                DifficultyState.DifficultyChanged -= OnDifficultyChanged;
        }

        public void Spawn()
        {
            ValidateRequiredDependencies();
            BuildObjects();
        }

        [Inject]
        public void Construct(DifficultyState difficultyState)
        {
            DifficultyState = difficultyState;
            CurrentSettings = DifficultyDatabase.
                GetSettings(DifficultyState.CurrentDifficulty);
            DifficultyState.DifficultyChanged += OnDifficultyChanged;
        }

        public virtual void ResetFactory(DifficultyLevel level)
        {
            ClearList();

            if (DifficultyState != null)
                DifficultyState.SetDifficulty(level);

            if (DifficultyDatabase != null)
                CurrentSettings = DifficultyDatabase.GetSettings(level);
        }

        public void NotifyObjectsChanged()
            => ListObjectsChanged?.Invoke(Objects);

        protected abstract void BuildObjects();

        protected virtual void OnDifficultyChanged(DifficultyLevel level)
            => ResetFactory(level);

        protected void Add(T @object)
        {
            if (@object != null)
                _objects.Add(@object);
        }

        protected virtual void ClearList()
        {
            foreach (var @object in Objects)
            {
                if (@object != null)
                    Destroy(@object.gameObject);
            }

            _objects.Clear();
        }

        private void ValidateRequiredDependencies()
        {
            if (Prefab == null)
                throw new ArgumentNullException(nameof(Prefab));

            if (DifficultyDatabase == null)
                throw new ArgumentNullException(nameof(DifficultyDatabase));
        }
    }
}