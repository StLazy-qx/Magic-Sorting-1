using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.UI.GamePanel;
using Assets.Source.Scripts.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.Source.Scripts.Enums;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class FinalGameSession : MonoBehaviour, IObjectInitilizable
    {
        [SerializeField] private BaseGameHandler _handler;
        [SerializeField] private Panel _endModelPanel;
        [SerializeField] private Image[] _roundRating;

        private Panel _currentPanel;
        private Panel _currentTutorialPanel;
        private RoundRating? _pendingRating;

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

        public void SetRoundRating(RoundRating rating)
        {
            _pendingRating = rating;
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

            ApplyRoundRating();
            _handler.PauseGame();
            _currentPanel.Open();
        }

        public void ShowEndModePanel()
        {
            ApplyRoundRating();
            _handler.PauseGame();
            _endModelPanel.Open();
        }

        private void ApplyRoundRating()
        {
            if (_pendingRating == null)
                return;

            int starsCount = GetStarsCount(_pendingRating.Value);

            for (int i = 0; i < _roundRating.Length; i++)
            {
                if (_roundRating[i] == null)
                    continue;

                _roundRating[i].enabled = i < starsCount;
            }

            _pendingRating = null;
        }

        private int GetStarsCount(RoundRating rating)
        {
            switch (rating)
            {
                case RoundRating.Bad: 
                    return 1;

                case RoundRating.Medium: 
                    return 2;

                case RoundRating.Good: 
                    return 3;

                default: 
                    return 0;
            }
        }
    }
}