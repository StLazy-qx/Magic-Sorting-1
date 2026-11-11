using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Tutorial;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class TutorialLevelCounter : MonoBehaviour
    {
        private const int EndTutorialRoundNumber = 1;

        private TutorialProgress _tutorialProgress;

        public event Action RoundChanged;

        public int RoundNumber { get; private set; }
        public bool IsLastRound => RoundNumber == EndTutorialRoundNumber;

        public void Initialize(TutorialProgress tutorialProgress)
        {
            Guard.NotNull(tutorialProgress, nameof(tutorialProgress));

            Unsubscribe();
            _tutorialProgress = tutorialProgress;
            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (_tutorialProgress == null)
                return;

            _tutorialProgress.RoundStarted -= OnRoundStarted;
            _tutorialProgress.RoundStarted += OnRoundStarted;
        }

        private void Unsubscribe()
        {
            if (_tutorialProgress == null)
                return;

            _tutorialProgress.RoundStarted -= OnRoundStarted;
        }

        private void OnRoundStarted(int roundNumber)
        {
            RoundNumber = roundNumber;
            RoundChanged?.Invoke();
        }
    }
}