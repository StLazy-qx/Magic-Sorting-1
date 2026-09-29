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
        private Panel _currentTutorialPanel;

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            Guard.NotNull(_handler, nameof(_handler));
            _currentPanel.Close();

            IsInitialized = true;
        }

        public void SetPanel(Panel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            _currentPanel = panel;
        }

        public void SetTutorialPanel(Panel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            _currentTutorialPanel = panel;
        }

        public void ShowEndRoundPanel()
        {
            if (_handler is TutorialGameHandler tutorialHandler)
            {
                if (tutorialHandler.IsTutorialCompleted)
                {
                    _handler.PauseGame();
                    _currentTutorialPanel.Open();

                    return;
                }
            }

            _handler.PauseGame();
            _currentPanel.Open();
        }
    }
}