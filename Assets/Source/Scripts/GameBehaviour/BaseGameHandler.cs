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
    /// <summary>
    /// Общая часть GameSessionHandler и TutorialGameHandler: запуск раунда, сброс сущностей, пауза, сцены.
    /// Сериализованные поля перенесены из наследников с теми же именами (ссылки в сценах сохраняются).
    /// Новые поля для назначения в сценах: _levelCounter, _vesselRoundController.
    /// Сложность берётся у LevelCounter, настройки у DifficultyDatabase (DifficultySetProvider больше не нужен).
    /// </summary>
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
        public GameMode Mode => _levelCounter.Mode;
        public bool IsLastRound => _levelCounter.IsLastRound;

        protected LevelCounter Counter => _levelCounter;

        [Inject]
        private void Construct(SceneLoader sceneLoader, Wallet wallet, DifficultyState difficultyState)
        {
            SceneLoader = sceneLoader;
            Wallet = wallet;
            DifficultyState = difficultyState;

            // Подписка здесь (до Awake), чтобы не пропустить первое RoundChanged из LevelCounter.Initialize
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

        /// <summary>Единственная точка передачи цветового контекста: фабрики берутся из сериализованных полей.</summary>
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

        /// <summary>
        /// Старт сессии (вызывается корнем в стадии сущностей, ПОСЛЕ сборки первого раунда):
        /// ровно то, что делал старый BaseGameHandler.Initialize. Следующие раунды сбрасывает ResetEntity.
        /// </summary>
        public void Initialize()
        {
            // Туториал в Start() уже мог поставить паузу (стартовая панель): её снимать нельзя
            if (IsPaused == false)
                ContinueGame();

            Wallet.Reset();
            _waitingPoint.Reset();

            IsInitialized = true;
        }

        /// <summary>
        /// Первый раунд сцены: тот же путь сборки (RoundLauncher), что и у всех следующих.
        /// Сброс сущностей и GameLaunching для него не вызываются:
        /// как и раньше, это делает Initialize() после сборки, а сущности сцены в этот момент ещё не инициализированы.
        /// </summary>
        public UniTask LaunchFirstRoundAsync()
            => LaunchRoundAsync(isFirstRound: true);

        /// <summary>Следующий раунд режима и его запуск.</summary>
        public void BeginRound()
        {
            if (_levelCounter.IsLastRound)
                return;

            // Снимаем паузу ДО смены раунда: слушатели RoundChanged (панели туториала) могут сразу
            // поставить паузу заново, и запуск раунда её уже не снимет.
            ContinueGame();

            if (_levelCounter.TryAdvance())
                LaunchRoundAsync(isFirstRound: false).Forget();
        }

        /// <summary>Перезапуск текущего раунда: индекс не меняется, слушатели узнают через RoundRestarted.</summary>
        public void ResetCurrentRound()
        {
            ContinueGame();
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

        /// <summary>Хук: счётчик сообщил о смене раунда (в том числе о первом).</summary>
        protected virtual void OnRoundChanged() { }

        /// <summary>Хук: наступил последний раунд фиксированного режима.</summary>
        protected virtual void OnLastRoundReached() { }

        /// <summary>Единственное место, где собирается раунд (первый, следующий и рестарт).</summary>
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
                ResetEntity();

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