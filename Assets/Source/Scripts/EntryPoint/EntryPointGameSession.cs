using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Zenject;


namespace Assets.Source.Scripts.EntryPoint
{
    public class EntryPointGameSession : MonoBehaviour
    {
        [SerializeField] private DifficultyDatabase _difficultyDatabase;
        [SerializeField] private PlatformGameAdapter _platformDependentSetter;
        [SerializeField] private ColorRandomizer _colorRandomizer;
        [SerializeField] private EntryColorListsFactory _entryColorListsFactory;
        [SerializeField] private ColumnsFactory _columnsFactory;
        [SerializeField] private VesselFactory _vesselFactory;
        [SerializeField] private StoreItemFactory _storeItemFactory;
        [SerializeField] private ColorColumnDistributor _columnDistributor;
        [SerializeField] private MonoBehaviour[] _objectsToInitializeMono;

        private DifficultyState _difficultyState;
        private DifficultySettings _currentSettings;
        
        private List<IObjectInitilizable> _objectsInitilizable = new();

        private void Awake()
        {
            ValidateDependencies();

            _currentSettings = _difficultyDatabase.GetSettings
                (_difficultyState.CurrentDifficulty);

            _colorRandomizer.CrateArrayColors(_currentSettings.ColorsCount);
            _platformDependentSetter.Initialize();
            _entryColorListsFactory.Initialize(
                _colorRandomizer.BeginColors, 
                _colorRandomizer.RemainingColors);
            _vesselFactory.InitRandomizer(_colorRandomizer);
            CollectInitializableObjects();
            StartCoroutine(SessionInitialize());
        }

        [Inject]
        private void Construct(DifficultyState difficultyState)
        {
            _difficultyState = difficultyState;
        }

        private void CollectInitializableObjects()
        {
            if (_objectsToInitializeMono.Length == 0)
                return;

            foreach (var mono in _objectsToInitializeMono)
            {
                if (mono is null)
                {
                    throw new ArgumentNullException(nameof(mono),
                        "Element inside _objectsToInitializeMono is null.");
                }

                if (mono is IObjectInitilizable initObj)
                    _objectsInitilizable.Add(initObj);
            }
        }

        private IEnumerator SessionInitialize()
        {
            yield return StartCoroutine(FactoryInitialize());
            yield return StartCoroutine(EntityInitialize());
        }

        private IEnumerator FactoryInitialize()
        {
            _vesselFactory.Spawn();
            _storeItemFactory.Spawn();

            yield return new WaitUntil(() => _vesselFactory.IsReady);

            if (_vesselFactory.Objects == null)
                throw new InvalidOperationException("VesselFactory.Objects is null.");

            if (_vesselFactory.Objects.Count > 0)
            {
                _columnsFactory.Initialize(
                    _vesselFactory.Objects,
                    _currentSettings.ColumnsCount,
                    _currentSettings.MaxCellsPerColumn);
                _columnsFactory.Spawn();
            }

            _columnDistributor.Distribute();
        }

        private IEnumerator EntityInitialize()
        {
            if (_objectsInitilizable.Count == 0)
                yield break;

            foreach (IObjectInitilizable currentObject in _objectsInitilizable)
            {
                currentObject.Initialize();

                if (currentObject is Inventory inventory)
                    inventory.SetScin();
            }

            yield return new WaitUntil(()
                => _objectsInitilizable.TrueForAll(currentObject => currentObject.IsInitialized));
        }

        private void ValidateDependencies()
        {
            if (_platformDependentSetter == null)
                throw new ArgumentNullException(nameof(_platformDependentSetter));

            if (_columnsFactory == null)
                throw new ArgumentNullException(nameof(_columnsFactory));

            if (_vesselFactory == null)
                throw new ArgumentNullException(nameof(_vesselFactory));

            if (_storeItemFactory == null)
                throw new ArgumentNullException(nameof(_storeItemFactory));

            if (_objectsToInitializeMono == null)
                throw new ArgumentNullException(nameof(_objectsToInitializeMono));
        }
    }
}