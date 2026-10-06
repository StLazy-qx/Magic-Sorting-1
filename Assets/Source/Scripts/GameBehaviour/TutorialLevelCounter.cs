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

        private void OnEnable()
        {
            _tutorialProgress.RoundStarted += OnRoundStarted;
        }

        private void OnDisable()
        {
            _tutorialProgress.RoundStarted -= OnRoundStarted;
        }

        public void Initialize(TutorialProgress tutorialProgress)
        {
            Guard.NotNull(tutorialProgress, nameof(tutorialProgress));

            _tutorialProgress = tutorialProgress;
        }

        private void OnRoundStarted(int roundNumber)
        {
            RoundNumber = roundNumber;

            RoundChanged?.Invoke();
        }
    }
}