using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Player;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.Composition_Root
{
    public class TutorialSessionCompositionRoot : CompositionRootBase
    {
        [Header("Scene")]
        [SerializeField] private DifficultyDatabase _difficultyDatabase;
        [SerializeField] private PlatformTutorialGameAdapter _platformSetter;
        [SerializeField] private ColorRandomizer _colorRandomizer;
        [SerializeField] private EntryColorListsFactory _entryColorListsFactory;
        [SerializeField] private ColumnsFactory _columnsFactory;
        [SerializeField] private VesselFactory _vesselFactory;
        [SerializeField] private ColorColumnDistributor _columnDistributor;
        [SerializeField] private TutorialGameHandler _tutorialGameHandler;

        private DifficultyState _difficultyState;
        private DifficultySettings _currentSettings;

        [Inject]
        private void Construct(
            DifficultyState difficultyState,
            SequenceDifficultyLevel sequenceDifficultyLevel)
        {
            Validator.AllNotNull(
                (difficultyState, nameof(difficultyState)),
                (sequenceDifficultyLevel, nameof(sequenceDifficultyLevel)));

            _difficultyState = difficultyState;
        }

        protected override void ValidateDependencies()
        {
            Validator.AllNotNull(
                (_difficultyDatabase, nameof(_difficultyDatabase)),
                (_platformSetter, nameof(_platformSetter)),
                (_colorRandomizer, nameof(_colorRandomizer)),
                (_entryColorListsFactory, nameof(_entryColorListsFactory)),
                (_columnsFactory, nameof(_columnsFactory)),
                (_vesselFactory, nameof(_vesselFactory)),
                (_columnDistributor, nameof(_columnDistributor)),
                (_tutorialGameHandler, nameof(_tutorialGameHandler)),
                (_difficultyState, nameof(_difficultyState)));
        }

        protected override void InitializeSceneDependenciesSync()
        {
            _tutorialGameHandler.Initialize();
            _tutorialGameHandler.RegisterFirstRound();

            _currentSettings = _difficultyDatabase.GetSettings(_difficultyState.CurrentDifficulty);
            Guard.NotNull(_currentSettings, nameof(_currentSettings));

            _colorRandomizer.SetDifficultySettings(_currentSettings.ColorsCount);
            _platformSetter.Initialize();
            _entryColorListsFactory.Initialize(
                _colorRandomizer.BeginColors,
                _colorRandomizer.RemainingColors);
            _vesselFactory.InitRandomizer(_colorRandomizer);
        }

        protected override IEnumerator InitializeFactoriesAsync()
        {
            _vesselFactory.Spawn();
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

        protected override void OnEntityInitialized(IObjectInitilizable obj)
        {
            if (obj is Inventory inventory)
                inventory.SetScin();
        }
    }
}
