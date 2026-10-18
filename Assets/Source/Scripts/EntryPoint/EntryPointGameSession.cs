using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.MagicCells;
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
        [SerializeField] private MagicMixer _magicMixer;
        [SerializeField] private MagicCellRouter _cellRouter;
        [SerializeField] private ClickModeSwitcher _clickModeSwitcher;
        [SerializeField] private MonoBehaviour[] _objectsToInitializeMono;

        private DifficultyState _difficultyState;
        private DifficultySettings _currentSettings;
        private ColorRecipes _colorRecipes;
        private ColorConverter _colorConverter;
        
        private List<IObjectInitilizable> _objectsInitilizable = new();

        private void Awake()
        {
            ValidateDependencies();

            _currentSettings = _difficultyDatabase.GetSettings
                (_difficultyState.CurrentDifficulty);
            _colorRecipes = new ColorRecipes();
            _colorConverter = new ColorConverter();

            _colorRandomizer.SetColorRecipes(_colorRecipes);
            _colorRandomizer.SetDifficultySettings(
				_currentSettings.ColorsCount, 
				_currentSettings.CountMixeColor);
            _platformDependentSetter.Initialize();
            _entryColorListsFactory.Initialize(
                _colorRandomizer.BeginColors, 
                _colorRandomizer.RemainingColors);
            _vesselFactory.InitRandomizer(_colorRandomizer);
            CollectInitializableObjects();
            _magicMixer.Initialize(_colorRecipes, _colorConverter);
            _magicMixer.MixFailed += _clickModeSwitcher.OnMixFailed;
            
            StartCoroutine(SessionInitialize());
        }
        
        private void OnEnable()
        {
        	if(_cellRouter != null &&
        		_clickModeSwitcher != null)
        	{
        		_cellRouter.CellSentToMixer += _clickModeSwitcher.OnMixerCellSent;
			}
		}
		
		private void OnDisable()
		{
			if(_cellRouter != null &&
        		_clickModeSwitcher != null)
        	{
        		_cellRouter.CellSentToMixer -= _clickModeSwitcher.OnMixerCellSent;
			}
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
			
			if (_cellRouter == null)
                throw new ArgumentNullException(nameof(_cellRouter));
			
			if (_clickModeSwitcher == null)
                throw new ArgumentNullException(nameof(_clickModeSwitcher));
			
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