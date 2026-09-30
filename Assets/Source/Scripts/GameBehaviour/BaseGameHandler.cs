using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.SceneManagement;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Enums;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.GameBehaviour
{
    public abstract class BaseGameHandler : MonoBehaviour, IObjectInitilizable
    {
        protected readonly int MainMenuIndex = 0;
        protected readonly int GameSessionIndex = 1;
        protected readonly int GamePause = 0;
        protected readonly int GameResume = 1;

        protected SceneLoader SceneLoader;
        protected Wallet Wallet;
        protected DifficultyState DifficultyState;

        public event Action<bool> PauseStateChanged;
        public event Action GameClosed;
        public event Action GameLaunching;

        public bool IsPaused { get; private set; }

        public bool IsInitialized { get; protected set; }

        //public void Initialize()
        //{
        //    if (SceneLoader == null)
        //        throw new ArgumentNullException(nameof(SceneLoader));

        //    if (Wallet == null)
        //        throw new InvalidOperationException(
        //            "Wallet not injected via Construct().");

        //    ContinueGame();
        //    Wallet.Reset();
        //    ExtendInitialize();

        //    IsInitialized = true;
        //}

        //public virtual void ContinueGame()
        //{
        //    IsPaused = false;
        //    PauseStateChanged?.Invoke(false);
        //}

        //public void PauseGame()
        //{
        //    IsPaused = true;
        //    PauseStateChanged?.Invoke(true);
        //}

        //public void ResumeGame()
        //{
        //    SceneLoader.LoadGameScene();
        //    IsPaused = false;
        //    PauseStateChanged?.Invoke(false);
        //}

        //public virtual void OpenMainMenu()
        //{
        //    SceneLoader.LoadMainMenuScene();
        //}

        //public void OpenTutorialScene()
        //{
        //    SceneLoader.LoadTutorialScene();
        //}

        //protected virtual void ExtendInitialize() { }

        //[Inject]
        //private void Construct(
        //    SceneLoader sceneLoader,
        //    Wallet wallet)
        //{
        //    SceneLoader = sceneLoader;
        //    Wallet = wallet;
        //}


        public void Initialize()
        {
            if (SceneLoader == null)
                throw new ArgumentNullException(nameof(SceneLoader));

            if (Wallet == null)
            {
                throw new InvalidOperationException(
                    "Wallet not injected via Construct().");
            }

            if (DifficultyState == null)
            {
                throw new InvalidOperationException(
                    "DifficultyState not injected via Construct().");
            }

            ContinueGame();
            Wallet.Reset();
            ExtendInitialize();

            IsInitialized = true;
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

        //Открытие сцен в отдельный класс

        public virtual void OpenMainMenu()
        {
            SceneLoader.LoadMainMenuScene();
        }

        // может стоит написать класс презентер, который подписывает методы переключения сцен на клики по кнопкам
        // чтобы не использовать методы в открытую 
        public void OpenTutorialScene()
        {
            SceneLoader.LoadTutorialScene();
        }

        protected virtual void ExtendInitialize() { }

        protected DifficultyLevel GetIncreasedDifficulty(DifficultyLevel current)
        {
            switch (current)
            {
                case DifficultyLevel.Easy:
                    return DifficultyLevel.Medium;

                case DifficultyLevel.Medium:
                    return DifficultyLevel.Hard;

                case DifficultyLevel.Hard:
                    return DifficultyLevel.Hard;

                default: return current;
            }
        }

        [Inject]
        private void Construct(
            SceneLoader sceneLoader,
            Wallet wallet,
            DifficultyState difficultyState)
        {
            SceneLoader = sceneLoader;
            Wallet = wallet;
            DifficultyState = difficultyState;
        }
    }
}
