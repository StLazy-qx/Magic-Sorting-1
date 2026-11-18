using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Player;
using System;

namespace Assets.Source.Scripts.GameBehaviour
{
    public interface IGameHandler : IObjectInitilizable
    {
        public event Action GameLaunching;
        public event Action<bool> PauseStateChanged;

        public bool IsPaused { get; }
        public bool IsLastRound { get; }
        public GameMode Mode { get; }

        public void ContinueGame();
        public void PauseGame();
        public void BeginRound();
        public void ResetCurrentRound();
    }
}