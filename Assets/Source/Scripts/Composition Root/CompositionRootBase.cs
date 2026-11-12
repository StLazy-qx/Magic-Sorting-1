using Assets.Source.Scripts.EntryPoint;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Composition_Root
{
    public abstract class CompositionRootBase : MonoBehaviour, ICompositionRoot
    {
        [SerializeField] private MonoBehaviour[] _objectsToInitialize;

        private readonly List<IObjectInitilizable> _initializableObjects = new();
        protected readonly CompositionValidator Validator = new();
        protected readonly InitializableObjectCollector Collector = new();

        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        protected IReadOnlyList<IObjectInitilizable> InitializableObjects => _initializableObjects;

        protected virtual void Awake()
        {
            ValidateDependencies();
            CollectInitializableObjects();
            InitializeSceneDependenciesSync();
            StartCoroutine(RunPipeline());
        }

        private IEnumerator RunPipeline()
        {
            yield return InitializeSceneDependenciesAsync();
            yield return InitializeFactoriesAsync();
            yield return InitializeEntitiesAsync();

            _isInitialized = true;

            OnPipelineCompleted();
        }

        protected abstract void ValidateDependencies();

        protected virtual void InitializeSceneDependenciesSync() { }

        protected virtual IEnumerator InitializeSceneDependenciesAsync() { yield break; }

        protected virtual IEnumerator InitializeFactoriesAsync() { yield break; }

        protected virtual IEnumerator InitializeEntitiesAsync()
        {
            if (_initializableObjects.Count == 0)
                yield break;

            foreach (IObjectInitilizable objectInitilizable in _initializableObjects)
            {
                objectInitilizable.Initialize();
                OnEntityInitialized(objectInitilizable);
            }

            yield return new WaitUntil(() =>
                _initializableObjects.TrueForAll(@object => @object.IsInitialized));
        }

        protected virtual void OnEntityInitialized(IObjectInitilizable obj) { }

        protected virtual void OnPipelineCompleted() { }

        protected virtual void CollectSceneInitializables() { }

        protected void AddInitializable(IObjectInitilizable obj)
        {
            if (obj != null && 
                _initializableObjects.Contains(obj) == false)
                _initializableObjects.Add(obj);
        }

        protected virtual void CollectInitializableObjects()
        {
            Collector.Collect(_objectsToInitialize, _initializableObjects);
            CollectSceneInitializables();
        }
    }
}
