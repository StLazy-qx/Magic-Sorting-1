using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.UI.GamePanel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;
using Assets.Source.Scripts.Extensions;

namespace Assets.Source.Scripts.Vessels
{
    public class VesselStateTracker : MonoBehaviour, IObjectInitilizable
    {
        [SerializeField] private FinalGameSession _finalGame;
        [SerializeField] private VesselCompletionEffecter _effecter;

        private Wallet _wallet;
        private Panel _currentPanel;
        private int _veselsCount;
        private IReadOnlyList<Vessel> _vessels;

        public event Action RoundOvering;
        public event Action VictoryAudioClipEnabled;

        public bool IsInitialized { get; private set; }

        private void OnDestroy()
        {
            if (_vessels == null)
                return;

            foreach (Vessel vessel in _vessels)
                vessel.RewardIssued -= OnPerformEffectCoroutine;
        }

        [Inject]
        public void Construct(Wallet wallet)
        {
            Guard.NotNull(wallet, nameof(wallet));

            _wallet = wallet;
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            ValidateInitializeArguments();

            IsInitialized = true;
        }

        public bool IsAllVesselsComplete()
        {
            return _veselsCount == _vessels.Count;
        }

        public void SetVesselsList(IReadOnlyList<Vessel> vessels)
        {
            ValidateVesselsList(vessels);

            _vessels = vessels;

            foreach (Vessel vessel in _vessels)
                vessel.RewardIssued += OnPerformEffectCoroutine;

            _effecter.Initialize(vessels.Count);
        }

        public void ApplyPanel(Panel panel)
        {
            Guard.NotNull(panel, nameof(panel));

            _currentPanel = panel;
        }

        private void OnPerformEffectCoroutine(Vector3 position, int points, Color color)
        {
            _veselsCount++;

            if (IsAllVesselsComplete())
                VictoryAudioClipEnabled?.Invoke();

            _wallet.AddPoints(points);
            StartCoroutine(PerformEffect(position, color));
        }

        private IEnumerator PerformEffect(Vector3 position, Color color)
        {
            RoundOvering.Invoke();
            
            yield return _effecter.PlayEffect(position, color);
            
            OnFixateVessel();
        }

        private void OnFixateVessel()
        {
            if (IsAllVesselsComplete())
            {
                _wallet.ConfirmPoints();
                _wallet.Reset();
                _currentPanel.Close();
                _finalGame.ShowEndRoundPanel();

                _veselsCount = 0;
            }
        }

        private void ValidateInitializeArguments()
        {
            Guard.IsTrue(_wallet != null, nameof(_wallet),
                "[VesselStateTracker] Wallet reference is missing. Did you forget Inject()?");
            Guard.IsTrue(_effecter != null, nameof(_effecter),
                "[VesselStateTracker] Effecter reference is missing in inspector.");
            Guard.IsTrue(_finalGame != null, nameof(_finalGame),
                "[VesselStateTracker] FinalGameSession reference is missing.");
            Guard.IsTrue(_vessels != null, nameof(_vessels),
                "[VesselStateTracker] Vessel list is not assigned. Call SetVesselsList() first.");
            Guard.IsTrue(_vessels.Count > 0, nameof(_vessels),
                "[VesselStateTracker] Vessel list is empty.");
            Guard.IsTrue(_vessels.All(v => v != null), nameof(_vessels),
                "[VesselStateTracker] Vessel list contains null entries.");
            Guard.IsTrue(_currentPanel != null, nameof(_currentPanel),
                "[VesselStateTracker] Panel is not assigned. Call ApplyPanel() first.");
        }

        private void ValidateVesselsList(IReadOnlyList<Vessel> vessels)
        {
            Guard.NotNull(vessels, nameof(vessels));
            Guard.NotNullOrEmpty(vessels, nameof(vessels));
            Guard.IsTrue(vessels.All(v => v != null), nameof(vessels),
                "[VesselStateTracker] Vessel list contains null entries");
        }
    }
}