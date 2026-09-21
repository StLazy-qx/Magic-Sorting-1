using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Zenject;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class TutorialGameHandler : BaseGameHandler
    {
        [SerializeField] private ColumnsFactory _columnsFactory;
        [SerializeField] private VesselFactory _vesselFactory;
        [SerializeField] private EntryColorListsFactory _entryColorListsFactory;
        [SerializeField] private ColorRandomizer _colorRandomizer;
        [SerializeField] private LevelCounter _levelCounter;
        [SerializeField] private WaitingPoint _waitingPoint;
        [SerializeField] private ClickModeSwitcher _clickImpactHandler;
        [SerializeField] private ColorColumnDistributor _columnDistributor;
        [SerializeField] private DifficultyDatabase _difficultyDatabase;

        private SequenceDifficultyLevel _sequenceDifficultyLevel;
        private DifficultyState _difficultyState;
        private DifficultySettings _currentSettings;
        
        private IReadOnlyList<DifficultyLevel> _tutorialSequence;
        private int _currentTutorialIndex;
        private bool _isTutorialCompleted;

        public event Action GameLaunching;
        public event Action TutorialCompleted;

        private void Awake()
        {
            ValidateObjects();
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
            _currentTutorialIndex = 0;
            _isTutorialCompleted = false;
            
            _waitingPoint.Reset();
        }

        public void BeginNewRound()
        {
            if (_isTutorialCompleted)
            {
                OnTutorialCompleted();
                return;
            }

            ChangeDifficultyBySequence();
            LaunchCurrentDifficulty();
        }

        public void ResetCurrentRound()
        {
            LaunchCurrentDifficulty();
        }

        private void ChangeDifficultyBySequence()
        {
            if (_currentTutorialIndex >= _tutorialSequence.Count)
            {
                _isTutorialCompleted = true;
                
                OnTutorialCompleted();
                
                return;
            }

            DifficultyLevel nextLevel = _tutorialSequence[_currentTutorialIndex];
            
            _difficultyState.SetDifficulty(nextLevel);
            
            _currentTutorialIndex++;
        }

        private void OnTutorialCompleted()
        {
            TutorialCompleted?.Invoke();
        }

        private void LaunchCurrentDifficulty()
        {
            _currentSettings = _difficultyDatabase
                .GetSettings(DifficultyState.CurrentDifficulty);

            _colorRandomizer.CrateArrayColors(_currentSettings.ColorsCount);
            StartRound();
            GameLaunching?.Invoke();
        }

        private void StartRound()
        {
            ContinueGame();
            ResetEntity();
            StartCoroutine(BeginRoundRoutine());
        }

        private IEnumerator BeginRoundRoutine()
        {
            ResetFactories();
            _vesselFactory.InitRandomizer(_colorRandomizer);
            _entryColorListsFactory.Initialize(
                _colorRandomizer.BeginColors,
                _colorRandomizer.RemainingColors);
            _vesselFactory.Spawn();

            yield return new WaitUntil(() => _vesselFactory.IsReady);

            if (_vesselFactory.Objects != null && _vesselFactory.Objects.Count > 0)
            {
                _columnsFactory.Initialize(
                    _vesselFactory.Objects,
                    _currentSettings.ColumnsCount,
                    _currentSettings.MaxCellsPerColumn);
                _columnsFactory.Spawn();
            }

            _columnDistributor.Distribute();
        }

        private void ResetEntity()
        {
            Wallet.Reset();
            _waitingPoint.Reset();
            _clickImpactHandler.Reset();
        }

        private void ResetFactories()
        {
            _entryColorListsFactory.Reset();
            _vesselFactory.ResetFactory(DifficultyState.CurrentDifficulty);
            _columnsFactory.ResetFactory(DifficultyState.CurrentDifficulty);
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