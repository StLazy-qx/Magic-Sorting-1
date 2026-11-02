using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.GameBehaviour;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.EntryPoint
{
    public class CompositionRootGameSession : MonoBehaviour
    {
        [SerializeField] private DifficultyDatabase _difficultyDatabase;
        [SerializeField] private PlatformGameAdapter _platformGameAdapter;
        [SerializeField] private ColorRandomizer _colorRandomizer;
        [SerializeField] private EntryColorListsFactory _entryColorListsFactory;
        [SerializeField] private ColumnsFactory _columnsFactory;
        [SerializeField] private VesselFactory _vesselFactory;
        [SerializeField] private StoreItemFactory _storeItemFactory;
        [SerializeField] private ColorColumnDistributor _columnDistributor;
        [SerializeField] private MagicMixer _magicMixer;
        [SerializeField] private MagicCellRouter _magicCellRouter;
        [SerializeField] private ClickModeSwitcher _clickModeSwitcher;
        [SerializeField] private GameSessionHandler _gameSessionHandler;
        [SerializeField] private MonoBehaviour[] _objectInitialize;

        private DifficultyState _difficultyState;
        private DifficultySettings _currentDifficultySettings;
        private SequenceDifficultyLevel _sequenceDifficultyLevel;
        private ColorCatalog _colorCatalog;
        private ColorRecipes _colorRecipes;
        private ColorConverter _colorConverter;
        private PlayerProgress _playerProgress;
        private List<IObjectInitilizable> _initializableObjects = new List<IObjectInitilizable>();

        public ColorRecipes ColorRecipes => _colorRecipes;
        public ColorConverter ColorConverter => _colorConverter;

        [Inject]
        private void Construct(
            DifficultyState difficultyState,
            PlayerProgress playerProgress,
            SequenceDifficultyLevel sequenceDifficultyLevel)
        {
            Guard.NotNull(difficultyState, nameof(difficultyState));
            Guard.NotNull(playerProgress, nameof(playerProgress));
            Guard.NotNull(sequenceDifficultyLevel, nameof(sequenceDifficultyLevel));

            _difficultyState = difficultyState;
            _playerProgress = playerProgress;
            _sequenceDifficultyLevel = sequenceDifficultyLevel;
        }

        private void Awake()
        {
            ValidateDependencies();

            _colorCatalog = new ColorCatalog();
            _colorRecipes = new ColorRecipes(_colorCatalog);
            _colorConverter = new ColorConverter(_colorCatalog);

            Guard.NotNull(_colorCatalog, nameof(_colorCatalog));
            Guard.NotNull(_colorRecipes, nameof(_colorRecipes));
            Guard.NotNull(_colorConverter, nameof(_colorConverter));
            _colorRandomizer.Initialize(
                _colorCatalog,
                _colorConverter,
                _colorRecipes);
            _platformGameAdapter.Initialize();
            _vesselFactory.InitRandomizer(_colorRandomizer);
            CollectInitializableObjects();
            _magicMixer.Initialize(_colorRecipes, _colorConverter);

            _magicMixer.MixFailed += _clickModeSwitcher.OnMixFailed;

            Guard.NotNull(_gameSessionHandler, nameof(_gameSessionHandler));
            _gameSessionHandler.SetLauncherDependencies(
                _columnsFactory,
                _vesselFactory,
                _entryColorListsFactory,
                _columnDistributor,
                _colorRecipes,
                _colorConverter);
            StartCoroutine(SessionInitialize());
        }

        private void OnEnable()
        {
            //Guard.NotNull(_magicCellRouter, nameof(_magicCellRouter));
            //Guard.NotNull(_clickModeSwitcher, nameof(_clickModeSwitcher));

            //_magicCellRouter.CellSentToMixer += _clickModeSwitcher.OnMixerCellSent;

            if (_magicCellRouter != null && _clickModeSwitcher != null)
                _magicCellRouter.CellSentToMixer += _clickModeSwitcher.OnMixerCellSent;

            if (_magicMixer != null && _clickModeSwitcher != null)
                _magicMixer.MixFailed += _clickModeSwitcher.OnMixFailed;
        }

        private void OnDisable()
        {
            //Guard.NotNull(_magicCellRouter, nameof(_magicCellRouter));
            //Guard.NotNull(_clickModeSwitcher, nameof(_clickModeSwitcher));

            //_magicCellRouter.CellSentToMixer -= _clickModeSwitcher.OnMixerCellSent;

            if (_magicCellRouter != null && _clickModeSwitcher != null)
                _magicCellRouter.CellSentToMixer -= _clickModeSwitcher.OnMixerCellSent;

            if (_magicMixer != null && _clickModeSwitcher != null)
                _magicMixer.MixFailed -= _clickModeSwitcher.OnMixFailed;
        }

        //private void OnDestroy()
        //{
        //    Guard.NotNull(_magicMixer, nameof(_magicMixer));
        //    Guard.NotNull(_clickModeSwitcher, nameof(_clickModeSwitcher));

        //    _magicMixer.MixFailed -= _clickModeSwitcher.OnMixFailed;
        //}

        public void RefreshRoundDistributor(
            ColorRandomizer colorRandomizer,
            DifficultySettings difficultySettings)
        {
            Guard.NotNull(colorRandomizer, nameof(colorRandomizer));
            Guard.NotNull(difficultySettings, nameof(difficultySettings));
            Guard.IsTrue(_colorRecipes != null && _colorConverter != null,
                "[CompositionRoot] ColorRecipes или ColorConverter " +
                "ещё не инициализированы.");
            Guard.NotNull(colorRandomizer.RoundColors, nameof(colorRandomizer.RoundColors));
            Guard.IsTrue(colorRandomizer.RoundColors.Count > 0,
                "[CompositionRoot] RoundColors пусты. " +
                "Распределитель не может быть настроен.");
            _columnDistributor.Initialize(
                _colorRecipes,
                _colorConverter,
                colorRandomizer.RoundColors,
                difficultySettings.CountMixeColor);
        }

        public void DistributeRoundColors()
        {
            Guard.NotNull(_columnDistributor, nameof(_columnDistributor));

            _columnDistributor.Distribute();
        }

        private void CollectInitializableObjects()
        {
            Guard.NotNull(_objectInitialize, nameof(_objectInitialize));

            if (_objectInitialize.Length == 0)
                return;

            foreach (MonoBehaviour monoBehaviour in _objectInitialize)
            {
                Guard.NotNull(monoBehaviour, nameof(_objectInitialize));

                if (monoBehaviour is IObjectInitilizable initializableObject
                    && _initializableObjects.Contains(initializableObject) == false)
                {
                    _initializableObjects.Add(initializableObject);
                }
            }
        }

        private IEnumerator SessionInitialize()
        {
            Guard.NotNull(_playerProgress, nameof(_playerProgress));
            Guard.NotNull(_sequenceDifficultyLevel, nameof(_sequenceDifficultyLevel));
            Guard.NotNull(_difficultyDatabase, nameof(_difficultyDatabase));

            if (_playerProgress.IsInitialized == false)
                _playerProgress.Initialize();

            yield return new WaitUntil(() => _playerProgress.IsInitialized);

            DifficultyLevel currentDifficultyLevel = ResolveRoundDifficulty();

            _currentDifficultySettings = _difficultyDatabase.GetSettings(currentDifficultyLevel);

            Guard.NotNull(_currentDifficultySettings, nameof(_currentDifficultySettings));
            _colorRandomizer.SetDifficultySettings(_currentDifficultySettings.ColorsCount);

            int foreignStrategyCount = 0;

            foreach (IColorStrategy colorStrategy in _colorRandomizer.RoundColors)
            {
                bool isOwnedByCatalog = false;

                foreach (IColorStrategy catalogStrategy in _colorCatalog.All)
                {
                    if (ReferenceEquals(catalogStrategy, colorStrategy))
                    {
                        isOwnedByCatalog = true;

                        break;
                    }
                }

                if (isOwnedByCatalog == false)
                    foreignStrategyCount++;
            }

            Guard.IsTrue(foreignStrategyCount == 0,
                nameof(_colorRandomizer),
                $"RoundColors содержат {foreignStrategyCount} стратегий из чужого ColorCatalog — " +
                "рецепты не будут совпадать по ссылке, колонки останутся пустыми.");
            _columnDistributor.Initialize(
                _colorRecipes,
                _colorConverter,
                _colorRandomizer.RoundColors,
                _currentDifficultySettings.CountMixeColor);
            _entryColorListsFactory.Initialize(
                _colorRandomizer.BeginColors,
                _colorRandomizer.RemainingColors);

            yield return StartCoroutine(FactoryInitialize());
            yield return StartCoroutine(EntityInitialize());
        }

        private DifficultyLevel ResolveRoundDifficulty()
        {
            Guard.NotNull(_playerProgress, nameof(_playerProgress));
            Guard.NotNull(_sequenceDifficultyLevel, nameof(_sequenceDifficultyLevel));

            if (_playerProgress.CurrentMode == GameMode.Tutorial)
                return _playerProgress.CurrentDifficultyLevel;

            _sequenceDifficultyLevel.SyncWithProgress();

            return _sequenceDifficultyLevel.GetCurrent();
        }

        private IEnumerator FactoryInitialize()
        {
            Guard.NotNull(_currentDifficultySettings, nameof(_currentDifficultySettings));
            Guard.NotNull(_vesselFactory, nameof(_vesselFactory));
            Guard.NotNull(_storeItemFactory, nameof(_storeItemFactory));
            Guard.NotNull(_columnsFactory, nameof(_columnsFactory));
            Guard.NotNull(_columnDistributor, nameof(_columnDistributor));

            _vesselFactory.Spawn();
            _storeItemFactory.Spawn();

            yield return new WaitUntil(() => _vesselFactory.IsReady);

            Guard.NotNull(_vesselFactory.Objects, nameof(_vesselFactory.Objects));

            if (_vesselFactory.Objects.Count > 0)
            {
                _columnsFactory.Initialize(
                    _vesselFactory.Objects,
                    _currentDifficultySettings.ColumnsCount,
                    _currentDifficultySettings.MaxCellsPerColumn);
                _columnsFactory.Spawn();
            }

            _columnDistributor.Distribute();
        }

        private IEnumerator EntityInitialize()
        {
            if (_initializableObjects.Count == 0)
                yield break;

            foreach (IObjectInitilizable initializableObject in _initializableObjects)
            {
                Guard.NotNull(initializableObject, nameof(_initializableObjects));

                initializableObject.Initialize();

                if (initializableObject is Inventory inventory)
                {
                    Guard.NotNull(inventory, nameof(inventory));
                    inventory.SetScin();
                }
            }

            yield return new WaitUntil(()
                => _initializableObjects.TrueForAll(o => o.IsInitialized));
        }

        private void ValidateDependencies()
        {
            Guard.NotNull(_difficultyDatabase, nameof(_difficultyDatabase));
            Guard.NotNull(_platformGameAdapter, nameof(_platformGameAdapter));
            Guard.NotNull(_colorRandomizer, nameof(_colorRandomizer));
            Guard.NotNull(_entryColorListsFactory, nameof(_entryColorListsFactory));
            Guard.NotNull(_columnsFactory, nameof(_columnsFactory));
            Guard.NotNull(_vesselFactory, nameof(_vesselFactory));
            Guard.NotNull(_storeItemFactory, nameof(_storeItemFactory));
            Guard.NotNull(_columnDistributor, nameof(_columnDistributor));
            Guard.NotNull(_magicMixer, nameof(_magicMixer));
            Guard.NotNull(_magicCellRouter, nameof(_magicCellRouter));
            Guard.NotNull(_clickModeSwitcher, nameof(_clickModeSwitcher));
            Guard.NotNull(_objectInitialize, nameof(_objectInitialize));
            Guard.NotNull(_gameSessionHandler, nameof(_gameSessionHandler));
        }
    }
}