using Assets.Source.Scripts.EntryPoint;
using System;

namespace Assets.Source.Scripts.GameBehaviour
{
    public interface IGameHandler
    {
        public bool IsInitialized { get; }

        public event Action<bool> PauseStateChanged;
        public event Action GameClosed;

        public void ContinueGame();
        public void PauseGame();
        //public void ResumeGame();
        //public void OpenMainMenu();
        public void QuitGame();
    }
}
