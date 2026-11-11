using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.GameBehaviour;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Zenject;
using Assets.Source.Scripts.Player;

namespace Assets.Source.Scripts.EntryPoint
{
    public class EntryPointTutorialSession : MonoBehaviour
    {
        [SerializeField] private DifficultyDatabase _difficultyDatabase;
        [SerializeField] private PlatformTutorialGameAdapter _platformSetter;
        [SerializeField] private ColorRandomizer _colorRandomizer;
        [SerializeField] private EntryColorListsFactory _entryColorListsFactory;
        [SerializeField] private ColumnsFactory _columnsFactory;
        [SerializeField] private VesselFactory _vesselFactory;
        [SerializeField] private ColorColumnDistributor _columnDistributor;
        [SerializeField] private TutorialGameHandler _tutorialGameHandler;
        [SerializeField] private MonoBehaviour[] _objectsToInitializeMono;

        private DifficultyState _difficultyState;
        private DifficultySettings _currentSettings;
        private ColorCatalog _colorCatalog;
        private ColorRecipes _colorRecipes;
        private ColorConverter _colorConverter;
        private List<IObjectInitilizable> _objectsInitilizable = new();

        [Inject]
        private void Construct(
            DifficultyState difficultyState,
            SequenceDifficultyLevel sequenceDifficultyLevel)
        {
            Guard.NotNull(difficultyState, nameof(difficultyState));
            Guard.NotNull(sequenceDifficultyLevel, nameof(sequenceDifficultyLevel));

            _difficultyState = difficultyState;
        }

        private void Awake()
        {
            ValidateDependencies();

            // 1. Цветовая подсистема — до любого использования ColorRandomizer.
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

            // 2. Платформа и фабрика сосудов.
            _platformSetter.Initialize();
            _vesselFactory.InitRandomizer(_colorRandomizer);

            // 3. Зависимости RoundLauncher для последующих раундов.
            _tutorialGameHandler.SetLauncherDependencies(
                _columnsFactory,
                _vesselFactory,
                _entryColorListsFactory,
                _columnDistributor,
                _colorRecipes,
                _colorConverter);

            // 4. Инициализация гейм-хендлера и регистрация первого раунда
            //    (RegisterFirstRound выставляет DifficultyState.CurrentDifficulty = Easy).
            _tutorialGameHandler.Initialize();
            _tutorialGameHandler.RegisterFirstRound();

            CollectInitializableObjects();

            StartCoroutine(SessionInitialize());
        }

        private void CollectInitializableObjects()
        {
            if (_objectsToInitializeMono == null || _objectsToInitializeMono.Length == 0)
                return;

            foreach (var mono in _objectsToInitializeMono)
            {
                if (mono is null)
                    throw new ArgumentNullException(
                        nameof(_objectsToInitializeMono),
                        "Element inside _objectsToInitializeMono is null.");

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
            // 5. Настройки берём уже для актуальной сложности (выставлена в RegisterFirstRound).
            _currentSettings = _difficultyDatabase.GetSettings(_difficultyState.CurrentDifficulty);

            Guard.NotNull(_currentSettings, nameof(_currentSettings));

            _colorRandomizer.SetDifficultySettings(_currentSettings.ColorsCount);

            // 6. Инициализируем распределитель ДО Distribute.
            _columnDistributor.Initialize(
                _colorRecipes,
                _colorConverter,
                _colorRandomizer.RoundColors,
                _currentSettings.CountMixeColor);

            _entryColorListsFactory.Initialize(
                _colorRandomizer.BeginColors,
                _colorRandomizer.RemainingColors);

            // 7. Спавн сосудов и колонок.
            _vesselFactory.Spawn();

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
            _tutorialGameHandler.NotifyRoundLaunched();
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
            Guard.NotNull(_difficultyDatabase, nameof(_difficultyDatabase));
            Guard.NotNull(_platformSetter, nameof(_platformSetter));
            Guard.NotNull(_colorRandomizer, nameof(_colorRandomizer));
            Guard.NotNull(_entryColorListsFactory, nameof(_entryColorListsFactory));
            Guard.NotNull(_columnsFactory, nameof(_columnsFactory));
            Guard.NotNull(_vesselFactory, nameof(_vesselFactory));
            Guard.NotNull(_columnDistributor, nameof(_columnDistributor));
            Guard.NotNull(_tutorialGameHandler, nameof(_tutorialGameHandler));
            Guard.NotNull(_objectsToInitializeMono, nameof(_objectsToInitializeMono));
        }
    }
}