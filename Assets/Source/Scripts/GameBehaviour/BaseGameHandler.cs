using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.SceneManagement;
using Assets.Source.Scripts.Vessels;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.GameBehaviour
{
    public abstract class BaseGameHandler : MonoBehaviour, IGameHandler
    {
        [Header("Round")]
        [SerializeField] private LevelCounter _levelCounter;
        [SerializeField] private DifficultyDatabase _difficultyDatabase;
        [SerializeField] private ColorRandomizer _colorRandomizer;

        [Header("Factories")]
        [SerializeField] private ColumnsFactory _columnsFactory;
        [SerializeField] private VesselFactory _vesselFactory;
        [SerializeField] private EntryColorListsFactory _entryColorListsFactory;
        [SerializeField] private ColorColumnDistributor _columnDistributor;

        [Header("Entities")]
        [SerializeField] private WaitingPoint _waitingPoint;
        [SerializeField] private ClickModeSwitcher _clickImpactHandler;
        [SerializeField] private VesselRoundController _vesselRoundController;

        private RoundLauncher _roundLauncher;

        protected SceneLoader SceneLoader;
        protected Wallet Wallet;
        protected DifficultyState DifficultyState;

        public event Action<bool> PauseStateChanged;
        public event Action GameLaunching;

        public bool IsPaused { get; private set; }
        public bool IsInitialized { get; protected set; }
        public bool IsLastRound => _levelCounter.IsLastRound;
        public GameMode Mode => _levelCounter.Mode;

        protected LevelCounter Counter => _levelCounter;

        [Inject]
        private void Construct(SceneLoader sceneLoader, Wallet wallet, DifficultyState difficultyState)
        {
            SceneLoader = sceneLoader;
            Wallet = wallet;
            DifficultyState = difficultyState;

            Guard.NotNull(_levelCounter, nameof(_levelCounter));

            _levelCounter.RoundChanged += OnRoundChanged;
            _levelCounter.LastRoundReached += OnLastRoundReached;
        }

        protected virtual void Awake()
        {
            ValidateObjects();
        }

        protected virtual void OnDestroy()
        {
            if (_levelCounter != null)
            {
                _levelCounter.RoundChanged -= OnRoundChanged;
                _levelCounter.LastRoundReached -= OnLastRoundReached;
            }

            _roundLauncher?.CancelCurrentLaunch();
        }

        public void SetColorContext(ColorRecipes recipes, ColorConverter converter)
        {
            _roundLauncher?.CancelCurrentLaunch();

            _roundLauncher = new RoundLauncher(
                _columnsFactory,
                _vesselFactory,
                _entryColorListsFactory,
                _columnDistributor,
                recipes,
                converter,
                _vesselRoundController.SetVesselsList);
        }

        public void Initialize()
        {
            ContinueGame();
            Wallet.Reset();
            _waitingPoint.Reset();

            IsInitialized = true;
        }

        public UniTask LaunchFirstRoundAsync()
            => LaunchRoundAsync(isFirstRound: true);

        public void BeginRound()
        {
            if (_levelCounter.TryAdvance())
                LaunchRoundAsync(isFirstRound: false).Forget();
        }

        public void ResetCurrentRound()
        {
            _levelCounter.RestartCurrentRound();
            LaunchRoundAsync(isFirstRound: false).Forget();
        }

        public virtual void ContinueGame()
        {
            IsPaused = false;

            PauseStateChanged?.Invoke(false);
        }

        public void PauseGame()
        {
            IsPaused = true;

            PauseStateChanged?.Invoke(true);
        }

        public void ResumeGame()
        {
            SceneLoader.LoadGameScene();

            IsPaused = false;

            PauseStateChanged?.Invoke(false);
        }

        public virtual void OpenMainMenu()
            => SceneLoader.LoadMainMenuScene();

        public void OpenTutorialScene()
            => SceneLoader.LoadTutorialScene();

        protected virtual void OnRoundChanged() { }

        protected virtual void OnLastRoundReached() { }

        private UniTask LaunchRoundAsync(bool isFirstRound)
        {
            DifficultyLevel level = _levelCounter.CurrentDifficulty;
            DifficultyState.SetDifficulty(level);

            DifficultySettings settings = _difficultyDatabase.GetSettings(level);

            if (settings == null)
            {
                Debug.LogError($"[{GetType().Name}] DifficultySettings для {level} не найдены.");

                return UniTask.CompletedTask;
            }

            if (_roundLauncher == null)
            {
                Debug.LogError(
                    $"[{GetType().Name}] RoundLauncher не создан. " +
                    "Composition Root должен вызвать SetColorContext().");

                return UniTask.CompletedTask;
            }

            _colorRandomizer.SetDifficultySettings(settings.ColorsCount);

            if (isFirstRound == false)
            {
                ContinueGame();
                ResetEntity();
            }

            UniTask launch = _roundLauncher.LaunchAsync(
                _colorRandomizer,
                settings,
                level,
                this.GetCancellationTokenOnDestroy());

            if (isFirstRound == false)
                GameLaunching?.Invoke();

            return launch;
        }

        private void ResetEntity()
        {
            Wallet.Reset();
            _waitingPoint.Reset();
            _clickImpactHandler.Reset();
        }

        private void ValidateObjects()
        {
            Guard.NotNull(_difficultyDatabase, nameof(_difficultyDatabase));
            Guard.NotNull(_colorRandomizer, nameof(_colorRandomizer));
            Guard.NotNull(_columnsFactory, nameof(_columnsFactory));
            Guard.NotNull(_vesselFactory, nameof(_vesselFactory));
            Guard.NotNull(_entryColorListsFactory, nameof(_entryColorListsFactory));
            Guard.NotNull(_columnDistributor, nameof(_columnDistributor));
            Guard.NotNull(_waitingPoint, nameof(_waitingPoint));
            Guard.NotNull(_clickImpactHandler, nameof(_clickImpactHandler));
            Guard.NotNull(_vesselRoundController, nameof(_vesselRoundController));
        }
    }
}