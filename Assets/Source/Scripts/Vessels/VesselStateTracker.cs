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
using System.Linq;

namespace Assets.Source.Scripts.Vessels
{
    public class VesselStateTracker : MonoBehaviour, IObjectInitilizable
    {
        [SerializeField] private FinalGameSession _finalGame;
        [SerializeField] private VesselCompletionEffecter _effecter;

        private bool _isModeCompleted;
        private int _veselsCount;
        private Wallet _wallet;
        private Panel _currentPanel;
        private SequenceDifficultyLevel _sequenceDifficultyLevel;
        private IReadOnlyList<Vessel> _vessels;

        private int _roundVersion;
        private int _pendingEffects;
        private bool _isRoundFinished;

        public event Action RoundOvering;
        public event Action VictoryAudioClipEnabled;

        public bool IsInitialized { get; private set; }

        private void OnDestroy()
        {
            UnsubscribeVessels();
        }

        [Inject]
        public void Construct(Wallet wallet,
            SequenceDifficultyLevel sequence)
        {
            Guard.NotNull(wallet, nameof(wallet));
            Guard.NotNull(sequence, nameof(sequence));

            _wallet = wallet;
            _sequenceDifficultyLevel = sequence;
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
            return _vessels != null
                && _vessels.Count > 0
                && _veselsCount >= _vessels.Count;
        }

        public void SetVesselsList(IReadOnlyList<Vessel> vessels)
        {
            ValidateVesselsList(vessels);
            ResetRound();

            _vessels = vessels;

            foreach (Vessel vessel in _vessels)
                vessel.RewardIssued += OnVesselRewardIssued;

            _effecter.Initialize(vessels.Count);
        }

        public void ResetRound()
        {
            StopAllCoroutines();
            UnsubscribeVessels();

            _roundVersion++;
            _veselsCount = 0;
            _pendingEffects = 0;
            _isRoundFinished = false;
            _vessels = null;
        }

        public void SetPanel(Panel panel)
        {
            Guard.NotNull(panel, nameof(panel));

            _currentPanel = panel;
        }

        private void OnVesselRewardIssued(Vector3 position, int points, Color color)
        {
            if (_isRoundFinished)
                return;

            _veselsCount++;
            _pendingEffects++;

            if (IsAllVesselsComplete())
                VictoryAudioClipEnabled?.Invoke();

            _wallet.AddPoints(points);
            StartCoroutine(PerformEffect(position, color, _roundVersion));
        }

        private IEnumerator PerformEffect(Vector3 position, Color color, int version)
        {
            RoundOvering?.Invoke();

            yield return _effecter.PlayEffect(position, color);

            if (version != _roundVersion)
                yield break;

            _pendingEffects--;

            TryFinishRound();
        }

        private void TryFinishRound()
        {
            if (_isRoundFinished || 
                _pendingEffects > 0 ||
                IsAllVesselsComplete() == false)
                return;

            _isRoundFinished = true;
            _isModeCompleted = false;
            _veselsCount = 0;

            _wallet.ConfirmPoints();
            _wallet.Reset();
            _currentPanel.Close();

            if (_sequenceDifficultyLevel.IsSequenceEnd)
            {
                _finalGame.ShowEndModePanel();

                return;
            }

            if (_sequenceDifficultyLevel.IsSequenceEnd == false)
                _finalGame.ShowEndRoundPanel();
        }

        private void UnsubscribeVessels()
        {
            if (_vessels == null)
                return;

            foreach (Vessel vessel in _vessels)
                vessel.RewardIssued -= OnVesselRewardIssued;
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

//namespace Assets.Source.Scripts.Vessels
//{
//    public class VesselStateTracker : MonoBehaviour, IObjectInitilizable
//    {
//        [SerializeField] private FinalGameSession _finalGame;
//        [SerializeField] private VesselCompletionEffecter _effecter;

//        private bool _isModeCompleted;
//        private int _veselsCount;
//        private Wallet _wallet;
//        private Panel _currentPanel;
//        private IReadOnlyList<Vessel> _vessels;

//        public event Action RoundOvering;
//        public event Action VictoryAudioClipEnabled;

//        public bool IsInitialized { get; private set; }

//        private void OnDestroy()
//        {
//            if (_vessels == null)
//                return;

//            foreach (Vessel vessel in _vessels)
//                vessel.RewardIssued -= OnPerformEffectCoroutine;
//        }

//        [Inject]
//        public void Construct(Wallet wallet)
//        {
//            Guard.NotNull(wallet, nameof(wallet));

//            _wallet = wallet;
//        }

//        public void Initialize()
//        {
//            if (IsInitialized)
//                return;

//            ValidateInitializeArguments();

//            IsInitialized = true;
//        }

//        public bool IsAllVesselsComplete()
//        {
//            return _veselsCount == _vessels.Count;
//        }

//        public void SetVesselsList(IReadOnlyList<Vessel> vessels)
//        {
//            ValidateVesselsList(vessels);

//            _vessels = vessels;

//            foreach (Vessel vessel in _vessels)
//                vessel.RewardIssued += OnPerformEffectCoroutine;

//            _effecter.Initialize(vessels.Count);
//        }

//        public void SetPanel(Panel panel)
//        {
//            Guard.NotNull(panel, nameof(panel));

//            _currentPanel = panel;
//        }

//        public void SetModeCompleted(bool isCompleted)
//        {
//            _isModeCompleted = isCompleted;
//        }

//        public void ResetModeCompleted()
//        {
//            _isModeCompleted = false;
//        }

//        public void CompleteAllVessels()
//        {
//            if (_vessels == null)
//                return;

//            foreach (Vessel vessel in _vessels)
//            {
//                if (vessel != null && vessel.IsActive && !vessel.IsFilled)
//                    vessel.CompleteVessel();
//            }
//        }

//        private void OnPerformEffectCoroutine(Vector3 position, int points, Color color)
//        {
//            _veselsCount++;

//            if (IsAllVesselsComplete())
//                VictoryAudioClipEnabled?.Invoke();

//            _wallet.AddPoints(points);
//            StartCoroutine(PerformEffect(position, color));
//        }

//        private IEnumerator PerformEffect(Vector3 position, Color color)
//        {
//            RoundOvering.Invoke();

//            yield return _effecter.PlayEffect(position, color);

//            OnFixateVessel();
//        }

//        private void OnFixateVessel()
//        {
//            if (IsAllVesselsComplete())
//            {
//                _wallet.ConfirmPoints();
//                _wallet.Reset();
//                _currentPanel.Close();
//                _finalGame.ShowEndRoundPanel(_isModeCompleted);

//                _veselsCount = 0;
//            }

//            //if (IsAllVesselsComplete())
//            //{
//            //    _wallet.ConfirmPoints();
//            //    _wallet.Reset();
//            //    _currentPanel.Close();
//            //    _finalGame.ShowEndRoundPanel();

//            //    _veselsCount = 0;
//            //}
//        }

//        private void ValidateInitializeArguments()
//        {
//            Guard.IsTrue(_wallet != null, nameof(_wallet),
//                "[VesselStateTracker] Wallet reference is missing. Did you forget Inject()?");
//            Guard.IsTrue(_effecter != null, nameof(_effecter),
//                "[VesselStateTracker] Effecter reference is missing in inspector.");
//            Guard.IsTrue(_finalGame != null, nameof(_finalGame),
//                "[VesselStateTracker] FinalGameSession reference is missing.");
//            Guard.IsTrue(_vessels != null, nameof(_vessels),
//                "[VesselStateTracker] Vessel list is not assigned. Call SetVesselsList() first.");
//            Guard.IsTrue(_vessels.Count > 0, nameof(_vessels),
//                "[VesselStateTracker] Vessel list is empty.");
//            Guard.IsTrue(_vessels.All(v => v != null), nameof(_vessels),
//                "[VesselStateTracker] Vessel list contains null entries.");
//            Guard.IsTrue(_currentPanel != null, nameof(_currentPanel),
//                "[VesselStateTracker] Panel is not assigned. Call ApplyPanel() first.");
//        }

//        private void ValidateVesselsList(IReadOnlyList<Vessel> vessels)
//        {
//            Guard.NotNull(vessels, nameof(vessels));
//            Guard.NotNullOrEmpty(vessels, nameof(vessels));
//            Guard.IsTrue(vessels.All(v => v != null), nameof(vessels),
//                "[VesselStateTracker] Vessel list contains null entries");
//        }
//    }
//}