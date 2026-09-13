using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Extensions;
using System.Collections;
using UnityEngine;
using System;
using Zenject;
using YG;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class GameSessionHandler : BaseGameHandler
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
        private bool _isInterstitialPending;
        private bool _isAdShowing = false;

        public event Action GameLaunching;
        private Action _pendingRoundAction = null;

        private void Awake()
        {
            ValidateObjects();
        }

        private void OnEnable()
        {
            YG2.onCloseInterAdv += OnCloseInterAdv;
            YG2.onCloseInterAdvWasShow += OnCloseInterAdvWasShow;
            YG2.onErrorInterAdv += OnErrorInterAdv;
        }

        private void OnDisable()
        {
            YG2.onCloseInterAdv -= OnCloseInterAdv;
            YG2.onCloseInterAdvWasShow -= OnCloseInterAdvWasShow;
            YG2.onErrorInterAdv -= OnErrorInterAdv;
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

        public void ShowInterstitialAd()
        {
            if (_isAdShowing)
                return;

            _isAdShowing = true;

            YG2.InterstitialAdvShow();
        }

        public void BeginNewRound()
        {
            ChangeDifficultyBySequence();
            LaunchCurrentDifficulty();
        }

        public void ResetCurrentRound()
        {
            LaunchCurrentDifficulty();
        }

        public void ShowInterstitial()
        {
            if (_isInterstitialPending)
                return;

            _isInterstitialPending = true;

            YG2.InterstitialAdvShow();
        }

        private void TryCompleteInterstitial()
        {
            if (_isInterstitialPending == false)
                return;

            _isInterstitialPending = false;

            BeginNewRound();
        }

        protected override void ExtendInitialize()
        {
            _waitingPoint.Reset();
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

        private void OnCloseInterAdv()
        {
            HandleAdClosed();
        }

        private void OnCloseInterAdvWasShow(bool wasShown)
        {
            HandleAdClosed();
        }

        private void OnErrorInterAdv()
        {
            HandleAdClosed();
        }

        private void HandleAdClosed()
        {
            _isAdShowing = false;

            if (_pendingRoundAction != null)
            {
                var action = _pendingRoundAction;
                _pendingRoundAction = null;
                action?.Invoke();
            }
        }

        private void ChangeDifficultyBySequence()
        {
            DifficultyLevel nextLevel = _sequenceDifficultyLevel.GetNext();

            DifficultyState.SetDifficulty(nextLevel);
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