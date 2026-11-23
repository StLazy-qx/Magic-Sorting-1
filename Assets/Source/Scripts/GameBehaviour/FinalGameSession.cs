using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.UI.GamePanel;
using UnityEngine;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class FinalGameSession : MonoBehaviour, IObjectInitilizable
    {
        [SerializeField] private BaseGameHandler _handler;

        private Panel _currentPanel;
        private Panel _currentEndModePanel;
        private Panel _currentTutorialPanel;

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            Guard.NotNull(_handler, nameof(_handler));

            if (_currentPanel != null)
                _currentPanel.Close();

            IsInitialized = true;
        }

        public void SetSessionEndPanels(Panel endRoundPanel, Panel endModePanel)
        {
            Guard.NotNull(endRoundPanel, nameof(endRoundPanel));
            Guard.NotNull(endModePanel, nameof(endModePanel));

            _currentPanel = endRoundPanel;
            _currentEndModePanel = endModePanel;
        }

        public void SetTutorialPanel(Panel panel)
        {
            Guard.NotNull(panel, nameof(panel));

            _currentTutorialPanel = panel;
        }

        public void ShowEndRoundPanel() 
            => PauseAndOpen(_currentPanel);

        public void ShowTutorialPanel() 
            => PauseAndOpen(_currentTutorialPanel);

        public void ShowEndModePanel() 
            => PauseAndOpen(_currentEndModePanel);

        private void PauseAndOpen(Panel panel)
        {
            Guard.NotNull(panel, nameof(panel));

            _handler.PauseGame();
            panel.Open();
        }
    }
}