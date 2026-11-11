using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.UI.GamePanel;
using Assets.Source.Scripts.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.Vessels
{
    public class VesselRoundController : MonoBehaviour, IObjectInitilizable
    {
        [SerializeField] private VesselStateTracker _tracker;
        [SerializeField] private VesselCompletionEffecter _effecter;
        [SerializeField] private FinalGameSession _finalGame;

        private Wallet _wallet;
        private SequenceDifficultyLevel _sequenceDifficultyLevel;
        private Panel _currentPanel;

        public event Action RoundOvering;
        public event Action VictoryAudioClipEnabled;

        public bool IsInitialized { get; private set; }

        [Inject]
        public void Construct(Wallet wallet, SequenceDifficultyLevel sequence)
        {
            Guard.NotNull(wallet, nameof(wallet));
            Guard.NotNull(sequence, nameof(sequence));

            _wallet = wallet;
            _sequenceDifficultyLevel = sequence;
        }

        private void OnEnable()
        {
            _tracker.VesselCompleted += OnVesselCompleted;
            _tracker.AllVesselsCompleted += OnAllVesselsCompleted;
            _tracker.RoundReset += OnRoundReset;
        }

        private void OnDisable()
        {
            _tracker.VesselCompleted -= OnVesselCompleted;
            _tracker.AllVesselsCompleted -= OnAllVesselsCompleted;
            _tracker.RoundReset -= OnRoundReset;
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            ValidateInitializeArguments();
            IsInitialized = true;
        }

        public void SetPanel(Panel panel)
        {
            Guard.NotNull(panel, nameof(panel));

            _currentPanel = panel;

            _finalGame.SetPanel(panel);
        }

        public void SetVesselsList(IReadOnlyList<Vessel> vessels)
        {
            _effecter.Initialize(vessels.Count);
            _tracker.SetVesselsList(vessels);
        }

        public bool IsAllVesselsComplete() => _tracker.IsAllVesselsComplete;

        private void OnRoundReset() => StopAllCoroutines();

        private void OnAllVesselsCompleted() => VictoryAudioClipEnabled?.Invoke();

        private void OnVesselCompleted(VesselCompletionInfo info)
        {
            _tracker.RegisterEffectStarted();
            _wallet.AddPoints(info.Points);
            StartCoroutine(PerformEffect(info));
        }

        private IEnumerator PerformEffect(VesselCompletionInfo info)
        {
            RoundOvering?.Invoke();

            yield return _effecter.PlayEffect(info.Position, info.Color);

            if (info.RoundVersion != _tracker.RoundVersion)
                yield break;

            _tracker.RegisterEffectFinished();

            if (!_tracker.IsReadyToFinish)
                yield break;

            _tracker.MarkRoundFinished();
            FinishRound();
        }

        private void FinishRound()
        {
            _wallet.ConfirmPoints();
            _wallet.Reset();
            _currentPanel.Close();

            if (_sequenceDifficultyLevel.IsSequenceEnd)
            {
                _finalGame.ShowEndModePanel();
                return;
            }

            if (IsTutorialCompleted())
            {
                _finalGame.ShowTutorialPanel();
                return;
            }

            _finalGame.ShowEndRoundPanel();
        }

        private bool IsTutorialCompleted()
            => _finalGame.Handler is TutorialGameHandler tutorial && tutorial.IsTutorialCompleted;

        private void ValidateInitializeArguments()
        {
            Guard.NotNull(_tracker, nameof(_tracker));
            Guard.NotNull(_effecter, nameof(_effecter));
            Guard.NotNull(_finalGame, nameof(_finalGame));
            Guard.IsTrue(_wallet != null, nameof(_wallet),
                "[VesselRoundController] Wallet reference is missing.");
            Guard.IsTrue(_sequenceDifficultyLevel != null, nameof(_sequenceDifficultyLevel),
                "[VesselRoundController] SequenceDifficultyLevel reference is missing.");
            Guard.IsTrue(_currentPanel != null, nameof(_currentPanel),
                "[VesselRoundController] Panel is not assigned. Call SetPanel() first.");
        }
    }
}