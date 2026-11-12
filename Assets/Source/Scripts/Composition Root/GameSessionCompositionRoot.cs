using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.Composition_Root
{
    public class GameSessionCompositionRoot : CompositionRootBase
    {
        [Header("Scene")]
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

        private DifficultyState _difficultyState;
        private PlayerProgress _playerProgress;
        private SequenceDifficultyLevel _sequenceDifficultyLevel;
        private DifficultySettings _currentDifficultySettings;
        private ColorContext _colorContext;

        public ColorContext ColorContext => _colorContext;
        public ColorRecipes ColorRecipes => _colorContext?.Recipes;
        public ColorConverter ColorConverter => _colorContext?.Converter;

        [Inject]
        private void Construct(
            DifficultyState difficultyState,
            PlayerProgress playerProgress,
            SequenceDifficultyLevel sequenceDifficultyLevel)
        {
            Validator.AllNotNull(
                (difficultyState, nameof(difficultyState)),
                (playerProgress, nameof(playerProgress)),
                (sequenceDifficultyLevel, nameof(sequenceDifficultyLevel)));

            _difficultyState = difficultyState;
            _playerProgress = playerProgress;
            _sequenceDifficultyLevel = sequenceDifficultyLevel;
        }

        protected override void ValidateDependencies()
        {
            Validator.AllNotNull(
                (_difficultyDatabase, nameof(_difficultyDatabase)),
                (_platformGameAdapter, nameof(_platformGameAdapter)),
                (_colorRandomizer, nameof(_colorRandomizer)),
                (_entryColorListsFactory, nameof(_entryColorListsFactory)),
                (_columnsFactory, nameof(_columnsFactory)),
                (_vesselFactory, nameof(_vesselFactory)),
                (_storeItemFactory, nameof(_storeItemFactory)),
                (_columnDistributor, nameof(_columnDistributor)),
                (_magicMixer, nameof(_magicMixer)),
                (_magicCellRouter, nameof(_magicCellRouter)),
                (_clickModeSwitcher, nameof(_clickModeSwitcher)),
                (_gameSessionHandler, nameof(_gameSessionHandler)),
                (_playerProgress, nameof(_playerProgress)),
                (_sequenceDifficultyLevel, nameof(_sequenceDifficultyLevel)));
        }

        protected override void InitializeSceneDependenciesSync()
        {
            _colorContext = new ColorContext();

            _colorRandomizer.Initialize(
                _colorContext.Catalog,
                _colorContext.Converter,
                _colorContext.Recipes);

            _platformGameAdapter.Initialize();
            _vesselFactory.InitRandomizer(_colorRandomizer);
            _magicMixer.Initialize(_colorContext.Recipes, _colorContext.Converter);

            // Двойная подписка MixFailed (тут и в OnEnable) была багом — оставляем только OnEnable.
            _gameSessionHandler.SetLauncherDependencies(
                _columnsFactory,
                _vesselFactory,
                _entryColorListsFactory,
                _columnDistributor,
                _colorContext.Recipes,
                _colorContext.Converter);
        }

        protected override IEnumerator InitializeSceneDependenciesAsync()
        {
            if (!_playerProgress.IsInitialized)
                _playerProgress.Initialize();

            yield return new WaitUntil(() => _playerProgress.IsInitialized);

            var level = ResolveRoundDifficulty();
            _currentDifficultySettings = _difficultyDatabase.GetSettings(level);
            Guard.NotNull(_currentDifficultySettings, nameof(_currentDifficultySettings));

            _colorRandomizer.SetDifficultySettings(_currentDifficultySettings.ColorsCount);

            ValidateRoundColorsOwnership(_colorRandomizer.RoundColors);

            _columnDistributor.Initialize(
                _colorContext.Recipes,
                _colorContext.Converter,
                _colorRandomizer.RoundColors,
                _currentDifficultySettings.CountMixeColor);

            _entryColorListsFactory.Initialize(
                _colorRandomizer.BeginColors,
                _colorRandomizer.RemainingColors);
        }

        protected override IEnumerator InitializeFactoriesAsync()
        {
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

        protected override void OnEntityInitialized(IObjectInitilizable obj)
        {
            if (obj is Inventory inventory)
                inventory.SetScin();
        }

        private DifficultyLevel ResolveRoundDifficulty()
        {
            if (_playerProgress.CurrentMode == GameMode.Tutorial)
                return _playerProgress.CurrentDifficultyLevel;

            _sequenceDifficultyLevel.SyncWithProgress();
            return _sequenceDifficultyLevel.GetCurrent();
        }

        private void ValidateRoundColorsOwnership(IEnumerable<IColorStrategy> roundColors)
        {
            int foreignCount = 0;

            foreach (var strategy in roundColors)
            {
                bool owned = false;
                foreach (var catalogStrategy in _colorContext.Catalog.All)
                {
                    if (ReferenceEquals(catalogStrategy, strategy))
                    {
                        owned = true;
                        break;
                    }
                }

                if (!owned)
                    foreignCount++;
            }

            Guard.IsTrue(
                foreignCount == 0,
                nameof(_colorRandomizer),
                $"RoundColors содержат {foreignCount} стратегий из чужого ColorCatalog — " +
                "рецепты не будут совпадать по ссылке, колонки останутся пустыми.");
        }

        private void OnEnable()
        {
            if (_magicCellRouter != null && _clickModeSwitcher != null)
                _magicCellRouter.CellSentToMixer += _clickModeSwitcher.OnMixerCellSent;

            if (_magicMixer != null && _clickModeSwitcher != null)
                _magicMixer.MixFailed += _clickModeSwitcher.OnMixFailed;
        }

        private void OnDisable()
        {
            if (_magicCellRouter != null && _clickModeSwitcher != null)
                _magicCellRouter.CellSentToMixer -= _clickModeSwitcher.OnMixerCellSent;

            if (_magicMixer != null && _clickModeSwitcher != null)
                _magicMixer.MixFailed -= _clickModeSwitcher.OnMixFailed;
        }

        public void RefreshRoundDistributor(
            ColorRandomizer colorRandomizer,
            DifficultySettings difficultySettings)
        {
            Guard.NotNull(colorRandomizer, nameof(colorRandomizer));
            Guard.NotNull(difficultySettings, nameof(difficultySettings));
            Guard.IsTrue(
                _colorContext != null && _colorContext.Recipes != null && _colorContext.Converter != null,
                "[GameSessionCompositionRoot] ColorContext ещё не инициализирован.");
            Guard.NotNull(colorRandomizer.RoundColors, nameof(colorRandomizer.RoundColors));
            Guard.IsTrue(
                colorRandomizer.RoundColors.Count > 0,
                "[GameSessionCompositionRoot] RoundColors пусты. " +
                "Распределитель не может быть настроен.");

            _columnDistributor.Initialize(
                _colorContext.Recipes,
                _colorContext.Converter,
                colorRandomizer.RoundColors,
                difficultySettings.CountMixeColor);
        }

        public void DistributeRoundColors()
        {
            Guard.NotNull(_columnDistributor, nameof(_columnDistributor));
            _columnDistributor.Distribute();
        }
    }
}
