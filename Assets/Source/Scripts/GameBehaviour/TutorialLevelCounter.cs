using Assets.Source.Scripts.Extensions;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class TutorialLevelCounter : MonoBehaviour
    {
        [SerializeField] private TutorialGameHandler _gameHandler;

        public event Action RoundChanged;

        public int RoundNumber { get; private set; }

        private void Awake()
        {
            Guard.NotNull(_gameHandler, nameof(_gameHandler));
        }

        private void OnEnable()
        {
            _gameHandler.TutorialRoundStarted += OnRoundStarted;
        }

        private void OnDisable()
        {
            _gameHandler.TutorialRoundStarted -= OnRoundStarted;
        }

        private void OnRoundStarted(int roundNumber)
        {
            RoundNumber = roundNumber;

            RoundChanged?.Invoke();
        }
    }
}
