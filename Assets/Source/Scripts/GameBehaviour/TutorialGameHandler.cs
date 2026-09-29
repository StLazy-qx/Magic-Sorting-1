using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Extensions;
using System.Collections.Generic;
using UnityEngine;
using System;
using Zenject;
using Cysharp.Threading.Tasks;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class TutorialGameHandler : BaseGameHandler, IGameHandler
    {
        [SerializeField] private ColumnsFactory _columnsFactory;
        [SerializeField] private VesselFactory _vesselFactory;
        [SerializeField] private EntryColorListsFactory _entryColorListsFactory;
        [SerializeField] private ColorRandomizer _colorRandomizer;
        [SerializeField] private WaitingPoint _waitingPoint;
        [SerializeField] private ClickModeSwitcher _clickImpactHandler;
        [SerializeField] private ColorColumnDistributor _columnDistributor;
        [SerializeField] private DifficultyDatabase _difficultyDatabase;

        private SequenceDifficultyLevel _sequenceDifficultyLevel;
        private DifficultyState _difficultyState;
        private DifficultySettings _currentSettings;
        
        private IReadOnlyList<DifficultyLevel> _tutorialSequence;
        private RoundLauncher _roundLauncher;
        private int _currentTutorialRoundIndex;
        private bool _isTutorialCompleted;

        public event Action GameLaunching;
        public event Action TutorialCompleted;
        public event Action<int> TutorialRoundStarted;

        public bool IsTutorialCompleted => _isTutorialCompleted;

        private void Awake()
        {
            ValidateObjects();

            _roundLauncher = new RoundLauncher(
                _columnsFactory,
                _vesselFactory,
                _entryColorListsFactory,
                _columnDistributor);
        }

        [Inject]
        private void Construct(
            DifficultyState difficultyState, 
            SequenceDifficultyLevel level)
        {
            Guard.NotNull(difficultyState, nameof(difficultyState));
            Guard.NotNull(level, nameof(level));

            _difficultyState = difficultyState;
            _sequenceDifficultyLevel = level;
        }

        protected override void ExtendInitialize()
        {
            _tutorialSequence = _sequenceDifficultyLevel.GetTutorialSequence();
            _currentTutorialRoundIndex = 0;
            _isTutorialCompleted = false;
            
            _waitingPoint.Reset();
        }

        public void BeginRound()
        {
            if (_isTutorialCompleted)
            {
                OnTutorialCompleted();

                return;
            }

            if (TryAdvanceToNextTutorialRound() == false)
                return;

            LaunchCurrentDifficulty();
        }

        public void ResetCurrentRound()
        {
            LaunchCurrentDifficulty();
        }

        private bool TryAdvanceToNextTutorialRound()
        {
            if (_tutorialSequence == null)
            {
                if (_sequenceDifficultyLevel == null)
                    return false;

                _tutorialSequence = _sequenceDifficultyLevel.
                    GetTutorialSequence();
                _currentTutorialRoundIndex = 0;
            }

            if (_sequenceDifficultyLevel == null 
                || _difficultyState == null)
                return false;

            if (_currentTutorialRoundIndex >= _tutorialSequence.Count)
            {
                _isTutorialCompleted = true;

                OnTutorialCompleted();

                return false;
            }

            DifficultyLevel nextLevel = 
                _tutorialSequence[_currentTutorialRoundIndex];

            _difficultyState.SetDifficulty(nextLevel);

            _currentTutorialRoundIndex++;

            TutorialRoundStarted?.Invoke(_currentTutorialRoundIndex);

            return true;
        }

        private void OnTutorialCompleted()
        {
            TutorialCompleted?.Invoke();
        }

        private void LaunchCurrentDifficulty()
        {
            _currentSettings = _difficultyDatabase
                .GetSettings(DifficultyState.CurrentDifficulty);

            if (_currentSettings == null)
                return;

            _colorRandomizer.CrateArrayColors(
                _currentSettings.ColorsCount);
            StartRound();
            GameLaunching?.Invoke();
        }

        private void StartRound()
        {
            ContinueGame();
            ResetEntity();
            _roundLauncher.LaunchAsync(
                _colorRandomizer,
                _currentSettings,
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