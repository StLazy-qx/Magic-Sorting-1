using System;

namespace Assets.Source.Scripts.GameBehaviour
{
    public interface IGameHandler
    {
        public bool IsInitialized { get; }

        public event Action<bool> PauseStateChanged;
        public event Action GameClosed;
        public event Action GameLaunching;

        public void ContinueGame();
        public void PauseGame();
    }
}
