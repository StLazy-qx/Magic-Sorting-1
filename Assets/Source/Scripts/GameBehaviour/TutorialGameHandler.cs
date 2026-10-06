using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Tutorial;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Extensions;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class TutorialGameHandler : BaseGameHandler, IGameHandler
    {
        [SerializeField] private ColumnsFactory _columnsFactory;
        [SerializeField] private VesselFactory _vesselFactory;
        [SerializeField] private EntryColorListsFactory _entryColorListsFactory;
        [SerializeField] private TutorialLevelCounter _tutorialLevelCounter;
        [SerializeField] private ColorRandomizer _colorRandomizer;
        [SerializeField] private WaitingPoint _waitingPoint;
        [SerializeField] private ClickModeSwitcher _clickImpactHandler;
        [SerializeField] private ColorColumnDistributor _columnDistributor;
        [SerializeField] private DifficultyDatabase _difficultyDatabase;

        private DifficultyState _difficultyState;
		private DifficultySetProvider _settingsProvider;
        private RoundLauncher _roundLauncher;
        private TutorialProgress _tutorialProgress;

        public event Action GameLaunching;
        public event Action TutorialCompleted;
        public event Action<int> TutorialRoundStarted;

        public bool IsTutorialCompleted => _tutorialProgress.IsLastRoundStarted;

        private void Awake()
        {
            ValidateObjects();

            _roundLauncher = new RoundLauncher(
                _columnsFactory,
                _vesselFactory,
                _entryColorListsFactory,
                _columnDistributor);
        }

        private void OnEnable()
        {
            _tutorialProgress.RoundStarted += OnTutorialRoundStarted;
            _tutorialProgress.Completed += OnTutorialProgressCompleted;
        }

        private void OnDisable()
        {
            _tutorialProgress.RoundStarted -= OnTutorialRoundStarted;
            _tutorialProgress.Completed -= OnTutorialProgressCompleted;
        }

        [Inject]
        private void Construct(
            DifficultyState difficultyState,
            SequenceDifficultyLevel difficultyLevel)
        {
            Guard.NotNull(difficultyState, nameof(difficultyState));
            Guard.NotNull(difficultyLevel, nameof(difficultyLevel));

            _difficultyState = difficultyState;
            _settingsProvider = new DifficultySetProvider(
                _difficultyDatabase,
                _difficultyState);
            _tutorialProgress = new TutorialProgress(difficultyLevel);

            _tutorialLevelCounter.Initialize(_tutorialProgress);
        }

        protected override void ExtendInitialize()
        {
            _waitingPoint.Reset();
        }

        public void RegisterFirstRound()
        {
            _tutorialProgress.Reset();
            TryAdvanceNextTutorialRound();
        }

        public void BeginRound()
        {
            if (TryAdvanceNextTutorialRound() == false)
                return;

            LaunchCurrentDifficulty();
        }

        public void ResetCurrentRound()
        {
            LaunchCurrentDifficulty();
        }

        private bool TryAdvanceNextTutorialRound()
        {
        	if (_tutorialProgress.TryGetNextLevel(out DifficultyLevel nextLevel) == false)
                return false;

            _difficultyState.SetDifficulty(nextLevel);

            return true;	
        }
        
        private void OnTutorialRoundStarted(int roundIndex)
        {
            TutorialRoundStarted?.Invoke(roundIndex);
        }
        
        private void OnTutorialProgressCompleted()
    	{
			TutorialCompleted?.Invoke();
   	    }

        private void LaunchCurrentDifficulty()
        {
        	DifficultySettings currentSettings = _settingsProvider.Current;

            if (currentSettings == null)
                return;
            
            _colorRandomizer.CrateArrayColors(currentSettings.ColorsCount);
        	StartRound(currentSettings);
            GameLaunching?.Invoke();
        }

		private void StartRound(DifficultySettings currentSettings)
        {
            ContinueGame();
            ResetEntity();
            _roundLauncher.LaunchAsync(
                _colorRandomizer,
                currentSettings,
                _tutorialLevelCounter,
                DifficultyState.CurrentDifficulty,
                this.GetCancellationTokenOnDestroy()).Forget();
        }
		
        private void ResetEntity()
        {
            Wallet.Reset();
            _waitingPoint.Reset();
            _clickImpactHandler.Reset();
        }

        private void ValidateObjects()
        {
            Guard.NotNull(_columnsFactory, nameof(_columnsFactory));
            Guard.NotNull(_vesselFactory, nameof(_vesselFactory));
            Guard.NotNull(_entryColorListsFactory, nameof(_entryColorListsFactory));
            Guard.NotNull(_colorRandomizer, nameof(_colorRandomizer));
            Guard.NotNull(_waitingPoint, nameof(_waitingPoint));
            Guard.NotNull(_clickImpactHandler, nameof(_clickImpactHandler));
            Guard.NotNull(_columnDistributor, nameof(_columnDistributor));
            Guard.NotNull(_difficultyDatabase, nameof(_difficultyDatabase));
        }
    }
}