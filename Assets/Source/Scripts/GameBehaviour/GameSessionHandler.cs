using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Extensions;
using UnityEngine;
using System;
using Zenject;
using YG;
using Cysharp.Threading.Tasks;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class GameSessionHandler : BaseGameHandler, IGameHandler
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
        private DifficultySetProvider _settingsProvider;
        private RoundLauncher _roundLauncher;
        private bool _isAdShowing;

        public event Action GameLaunching;

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
            _settingsProvider = new DifficultySetProvider(
                _difficultyDatabase,
                _difficultyState);
        }

        public void ShowInterstitialAdvertising()
        {
            if (_isAdShowing)
                return;

            _isAdShowing = true;

            YG2.InterstitialAdvShow();
        }

        public void BeginRound()
        {
            ChangeDifficultyBySequence();
            LaunchCurrentDifficulty();
        }

        public void ResetCurrentRound()
        {
            LaunchCurrentDifficulty();
        }

        protected override void ExtendInitialize()
        {
            _waitingPoint.Reset();
        }

        private void LaunchCurrentDifficulty()
        {
            DifficultySettings currentSettings = _settingsProvider.Current;

            if (currentSettings == null)
                return;

            _colorRandomizer.SetDifficultySettings(
                currentSettings.ColorsCount,
                currentSettings.CountMixeColor);
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
                DifficultyState.CurrentDifficulty,
                this.GetCancellationTokenOnDestroy()).Forget();
        }

        private void ResetEntity()
        {
            Wallet.Reset();
            _waitingPoint.Reset();
            _clickImpactHandler.Reset();
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
        }

        private void ChangeDifficultyBySequence()
        {
            DifficultyLevel nextLevel = _sequenceDifficultyLevel.GetNext();

            DifficultyState.SetDifficulty(nextLevel);
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