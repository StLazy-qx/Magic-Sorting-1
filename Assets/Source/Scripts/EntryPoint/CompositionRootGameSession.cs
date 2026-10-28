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
using System;

namespace Assets.Source.Scripts.EntryPoint
{
    public class CompositionRootGameSession : MonoBehaviour
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
        [SerializeField] private GameSessionHandler _gameSessionHandler;
        [SerializeField] private MonoBehaviour[] _objectsToInitializeMono;

        private DifficultyState _difficultyState;
        private DifficultySettings _currentSettings;
        private SequenceDifficultyLevel _sequenceDifficulty;
        private ColorCatalog _colorCatalog;
        private ColorRecipes _colorRecipes;
        private ColorConverter _colorConverter;
        private PlayerProgress _playerProgress;
        private List<IObjectInitilizable> _objectsInitilizable = new();

        public ColorRecipes ColorRecipes => _colorRecipes;
        public ColorConverter ColorConverter => _colorConverter;

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
            _platformDependentSetter.Initialize();
            _vesselFactory.InitRandomizer(_colorRandomizer);
            CollectInitializableObjects();
            _magicMixer.Initialize(_colorRecipes, _colorConverter);

            _magicMixer.MixFailed += _clickModeSwitcher.OnMixFailed;

            if (_gameSessionHandler != null)
            {
                _gameSessionHandler.SetLauncherDependencies(
                    _columnsFactory,
                    _vesselFactory,
                    _entryColorListsFactory,
                    _columnDistributor,
                    _colorRecipes,
                    _colorConverter);
            }

            StartCoroutine(SessionInitialize());
        }

        private void OnEnable()
        {
            if (_cellRouter != null && _clickModeSwitcher != null)
                _cellRouter.CellSentToMixer += _clickModeSwitcher.OnMixerCellSent;
        }

        private void OnDisable()
        {
            if (_cellRouter != null && _clickModeSwitcher != null)
                _cellRouter.CellSentToMixer -= _clickModeSwitcher.OnMixerCellSent;
        }

        private void OnDestroy()
        {
            if (_magicMixer != null && _clickModeSwitcher != null)
                _magicMixer.MixFailed -= _clickModeSwitcher.OnMixFailed;
        }

        [Inject]
        private void Construct(
            DifficultyState difficultyState,
            PlayerProgress playerProgress,
            SequenceDifficultyLevel sequenceDifficulty)
        {
            Guard.NotNull(difficultyState, nameof(difficultyState));
            Guard.NotNull(playerProgress, nameof(playerProgress));
            Guard.NotNull(sequenceDifficulty, nameof(sequenceDifficulty));

            _difficultyState = difficultyState;
            _playerProgress = playerProgress;
            _sequenceDifficulty = sequenceDifficulty;
        }

        public void RefreshRoundDistributor(
            ColorRandomizer colorRandomizer,
            DifficultySettings settings)
        {
            Guard.NotNull(colorRandomizer, nameof(colorRandomizer));
            Guard.NotNull(settings, nameof(settings));

            if (_colorRecipes == null || _colorConverter == null)
            {
                throw new InvalidOperationException(
                    "[CompositionRoot] ColorRecipes или ColorConverter " +
                    "ещё не инициализированы.");
            }

            if (colorRandomizer.RoundColors == null ||
                colorRandomizer.RoundColors.Count == 0)
            {
                throw new InvalidOperationException(
                    "[CompositionRoot] RoundColors пусты. " +
                    "Распределитель не может быть настроен.");
            }

            _columnDistributor.Initialize(
                _colorRecipes,
                _colorConverter,
                colorRandomizer.RoundColors,
                settings.CountMixeColor);
        }

        public void DistributeRoundColors()
        {
            _columnDistributor.Distribute();
        }

        private void CollectInitializableObjects()
        {
            Guard.NotNull(_objectsToInitializeMono, nameof(_objectsToInitializeMono));

            if (_objectsToInitializeMono.Length == 0)
                return;

            foreach (var mono in _objectsToInitializeMono)
            {
                Guard.NotNull(mono, nameof(_objectsToInitializeMono));

                if (mono is IObjectInitilizable initObj
                    && _objectsInitilizable.Contains(initObj) == false)
                {
                    _objectsInitilizable.Add(initObj);
                }
            }
        }

        private IEnumerator SessionInitialize()
        {
            if (_playerProgress == null)
                yield break;

            if (_playerProgress.IsInitialized == false)
                _playerProgress.Initialize();

            yield return new WaitUntil(() => _playerProgress.IsInitialized);

            DifficultyLevel roundDifficulty = ResolveRoundDifficulty();

            _currentSettings = _difficultyDatabase.GetSettings(roundDifficulty);

            Guard.NotNull(_currentSettings, nameof(_currentSettings));
            _colorRandomizer.SetDifficultySettings(_currentSettings.ColorsCount);

            int foreign = 0;

            foreach (var c in _colorRandomizer.RoundColors)
            {
                bool owned = false;

                foreach (var own in _colorCatalog.All)
                {
                    if (ReferenceEquals(own, c))
                    {
                        owned = true;
                        break;
                    }
                }

                if (owned == false)
                    foreign++;
            }

            Guard.IsTrue(foreign == 0,
                nameof(_colorRandomizer),
                $"RoundColors содержат {foreign} стратегий из чужого ColorCatalog — " +
                "рецепты не будут совпадать по ссылке, колонки останутся пустыми.");

            _columnDistributor.Initialize(
                _colorRecipes,
                _colorConverter,
                _colorRandomizer.RoundColors,
                _currentSettings.CountMixeColor);
            _entryColorListsFactory.Initialize(
                _colorRandomizer.BeginColors,
                _colorRandomizer.RemainingColors);

            yield return StartCoroutine(FactoryInitialize());
            yield return StartCoroutine(EntityInitialize());
        }

        private DifficultyLevel ResolveRoundDifficulty()
        {
            if (_playerProgress.CurrentMode == GameMode.Tutorial)
                return _playerProgress.CurrentDifficultyLevel;

            _sequenceDifficulty.SyncWithProgress();

            return _sequenceDifficulty.GetNext();
        }

        private IEnumerator FactoryInitialize()
        {
            _vesselFactory.Spawn();
            _storeItemFactory.Spawn();

            yield return new WaitUntil(() => _vesselFactory.IsReady);

            Guard.NotNull(_vesselFactory.Objects, nameof(_vesselFactory.Objects));

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
                => _objectsInitilizable.TrueForAll(o => o.IsInitialized));
        }

        private void ValidateDependencies()
        {
            Guard.NotNull(_difficultyDatabase, nameof(_difficultyDatabase));
            Guard.NotNull(_platformDependentSetter, nameof(_platformDependentSetter));
            Guard.NotNull(_colorRandomizer, nameof(_colorRandomizer));
            Guard.NotNull(_entryColorListsFactory, nameof(_entryColorListsFactory));
            Guard.NotNull(_columnsFactory, nameof(_columnsFactory));
            Guard.NotNull(_vesselFactory, nameof(_vesselFactory));
            Guard.NotNull(_storeItemFactory, nameof(_storeItemFactory));
            Guard.NotNull(_columnDistributor, nameof(_columnDistributor));
            Guard.NotNull(_magicMixer, nameof(_magicMixer));
            Guard.NotNull(_cellRouter, nameof(_cellRouter));
            Guard.NotNull(_clickModeSwitcher, nameof(_clickModeSwitcher));
            Guard.NotNull(_objectsToInitializeMono, nameof(_objectsToInitializeMono));
            Guard.NotNull(_gameSessionHandler, nameof(_gameSessionHandler));
        }
    }
}