using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Composition_Root;
using UnityEngine;
using System;
using Zenject;
using YG;
using Cysharp.Threading.Tasks;
using Assets.Source.Scripts.Player;

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
        [SerializeField] private GameSessionCompositionRoot _compositionRoot;

        private SequenceDifficultyLevel _sequenceDifficultyLevel;
        private DifficultyState _difficultyState;
        private DifficultySettingsProvider _settingsProvider;
        private RoundLauncher _roundLauncher;
        private PlayerProgress _playerProgress;
        private bool _isAdShowing;

        public event Action GameLaunching;

        private void Awake()
        {
            ValidateObjects();
        }

        public void SetLauncherDependencies(
            ColumnsFactory columnsFactory,
            VesselFactory vesselFactory,
            EntryColorListsFactory entryColorListsFactory,
            ColorColumnDistributor columnDistributor,
            ColorRecipes recipes,
            ColorConverter converter)
        {
            Guard.NotNull(columnsFactory, nameof(columnsFactory));
            Guard.NotNull(vesselFactory, nameof(vesselFactory));
            Guard.NotNull(entryColorListsFactory, nameof(entryColorListsFactory));
            Guard.NotNull(columnDistributor, nameof(columnDistributor));
            Guard.NotNull(recipes, nameof(recipes));
            Guard.NotNull(converter, nameof(converter));

            _roundLauncher = new RoundLauncher(
                columnsFactory,
                vesselFactory,
                entryColorListsFactory,
                columnDistributor,
                recipes,
                converter);
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
            SequenceDifficultyLevel level,
            PlayerProgress playerProgress)
        {
            Guard.NotNull(difficultyState, nameof(difficultyState));
            Guard.NotNull(level, nameof(level));

            _difficultyState = difficultyState;
            _sequenceDifficultyLevel = level;
            _settingsProvider = new DifficultySettingsProvider(
                _difficultyDatabase,
                _difficultyState);
            _playerProgress = playerProgress;
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
            ChangeDifficultySequence();
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

            _colorRandomizer.SetDifficultySettings(currentSettings.ColorsCount);
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
            _compositionRoot.ResetRoundState();
        }

        private void OnCloseInterAdv()
            => HandleAdClosed();

        private void OnCloseInterAdvWasShow(bool wasShown)
            => HandleAdClosed();

        private void OnErrorInterAdv()
            => HandleAdClosed();

        private void HandleAdClosed()
        {
            _isAdShowing = false;
        }

        private void ChangeDifficultySequence()
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
            Guard.NotNull(_compositionRoot, nameof(_compositionRoot));
        }
    }
}