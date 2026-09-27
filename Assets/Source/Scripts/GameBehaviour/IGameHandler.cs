using System;

namespace Assets.Source.Scripts.GameBehaviour
{
    public interface IGameHandler
    {
        public bool IsInitialized { get; }

        public void ContinueGame();
        public void PauseGame();
        void BeginRound();
        void ResetCurrentRound();
    }
}
