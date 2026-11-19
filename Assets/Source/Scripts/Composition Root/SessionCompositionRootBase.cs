using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.Player;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Assets.Source.Scripts.Composition_Root
{
    public abstract class SessionCompositionRootBase : CompositionRootBase
    {
        [SerializeField] private ColorRandomizer _colorRandomizer;
        [SerializeField] private LevelCounter _levelCounter;
        [SerializeField] private RoundSequenceTracker _roundTracker;
        [SerializeField] private BaseGameHandler _gameHandler;

        private PlayerProgress _playerProgress;

        public ColorContext ColorContext { get; private set; }
        public ColorRecipes ColorRecipes => ColorContext?.Recipes;
        public ColorConverter ColorConverter => ColorContext?.Converter;

        protected PlayerProgress Progress => _playerProgress;
        protected BaseGameHandler GameHandler => _gameHandler;

        [Inject]
        private void ConstructSession(PlayerProgress playerProgress)
        {
            _playerProgress = playerProgress;
        }

        protected abstract GameMode ResolveMode();

        protected virtual void ValidateSessionDependencies() { }
        protected virtual void InitializeSessionSync() { }
        protected virtual void SpawnAdditionalFactories() { }

        protected sealed override void ValidateDependencies()
        {
            Validator.AllNotNull(
                (_colorRandomizer, nameof(_colorRandomizer)),
                (_levelCounter, nameof(_levelCounter)),
                (_roundTracker, nameof(_roundTracker)),
                (_gameHandler, nameof(_gameHandler)));

            ValidateSessionDependencies();
        }

        protected sealed override void InitializeSceneDependenciesSync()
        {
            ColorContext = new ColorContext(new ColorCatalog());

            _colorRandomizer.Initialize(
                ColorContext.Catalog,
                ColorContext.Converter,
                ColorContext.Recipes);

            _gameHandler.SetColorContext(ColorContext.Recipes, ColorContext.Converter);

            InitializeSessionSync();
        }

        protected sealed override IEnumerator InitializeSceneDependenciesAsync()
        {
            if (_playerProgress.IsInitialized == false)
                _playerProgress.Initialize();

            yield return new WaitUntil(() => _playerProgress.IsInitialized);

            _levelCounter.Initialize(ResolveMode());
            _roundTracker.Initialize(_levelCounter);
        }

        protected sealed override IEnumerator InitializeFactoriesAsync()
        {
            SpawnAdditionalFactories();

            // Первый раунд собирается тем же кодом, что и все следующие
            yield return UniTask.ToCoroutine(() => _gameHandler.LaunchFirstRoundAsync());

            ValidateRoundColorsOwnership(_colorRandomizer.RoundColors);
        }

        protected override void CollectSceneInitializables()
        {
            AddInitializable(_gameHandler);
        }

        protected override void OnEntityInitialized(IObjectInitilizable obj)
        {
            if (obj is Inventory inventory)
                inventory.SetScin();
        }

        private void ValidateRoundColorsOwnership(IEnumerable<IColorStrategy> roundColors)
        {
            int foreignCount = 0;

            foreach (var strategy in roundColors)
            {
                bool owned = false;

                foreach (var catalogStrategy in ColorContext.Catalog.All)
                {
                    if (ReferenceEquals(catalogStrategy, strategy))
                    {
                        owned = true;
                        break;
                    }
                }

                if (owned == false)
                    foreignCount++;
            }

            Guard.IsTrue(
                foreignCount == 0,
                nameof(_colorRandomizer),
                $"RoundColors содержат {foreignCount} стратегий из чужого ColorCatalog — " +
                "рецепты не будут совпадать по ссылке, колонки останутся пустыми.");
        }
    }
}