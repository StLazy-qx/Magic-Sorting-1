using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.SceneManagement;
using Assets.Source.Scripts.Enums;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.GameBehaviour
{
    public abstract class BaseGameHandler : MonoBehaviour, IObjectInitilizable, IGameHandler
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

        public bool IsInitialized { get; protected set; }

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
            PauseStateChanged?.Invoke(false);

            Time.timeScale = GameResume;
        }

        public void PauseGame()
        {
            //Time.timeScale = GamePause;

            PauseStateChanged?.Invoke(true);
        }

        public void ResumeGame()
        {
            SceneLoader.LoadGameScene();
        }

        public virtual void OpenMainMenu()
        {
            SceneLoader.LoadMainMenu();
        }

        public virtual void QuitGame()
        {
            GameClosed?.Invoke();
            Application.Quit();
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
