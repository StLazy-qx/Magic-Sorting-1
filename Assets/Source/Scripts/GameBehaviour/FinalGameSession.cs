using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.UI.GamePanel;
using Assets.Source.Scripts.Extensions;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class FinalGameSession : MonoBehaviour, IObjectInitilizable
    {
        [SerializeField] private BaseGameHandler _handler;

        private Panel _currentPanel;

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            Guard.NotNull(_handler, nameof(_handler));
            _currentPanel.Close();

            IsInitialized = true;
        }

        public void ApplyPanel(Panel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            _currentPanel = panel;
        }

        public void ShowEndRoundPanel()
        {
            _handler.PauseGame();
            _currentPanel.Open();
        }
    }
}