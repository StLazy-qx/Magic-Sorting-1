using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Scripts.Vessels
{
    public class VesselStateTracker : MonoBehaviour, IObjectInitilizable
    {
        private IReadOnlyList<Vessel> _vessels;
        private int _completedCount;
        private int _pendingEffects;
        private int _roundVersion;
        private bool _isRoundFinished;

        public event Action<VesselCompletionInfo> VesselCompleted;
        public event Action AllVesselsCompleted;
        public event Action RoundReset;

        public int RoundVersion => _roundVersion;
        public bool IsRoundFinished => _isRoundFinished;
        public int PendingEffects => _pendingEffects;
        public int TotalCount => _vessels?.Count ?? 0;
        public int CompletedCount => _completedCount;

        public bool IsAllVesselsComplete =>
            _vessels != null && _vessels.Count > 0 && _completedCount >= _vessels.Count;

        public bool IsReadyToFinish =>
            _isRoundFinished == false && 
            _pendingEffects == 0 && 
            IsAllVesselsComplete;

        public bool IsInitialized { get; private set; }

        public void Initialize() 
            => IsInitialized = true;

        public void SetVesselsList(IReadOnlyList<Vessel> vessels)
        {
            ValidateVesselsList(vessels);
            UnsubscribeVessels();

            _vessels = vessels;

            foreach (Vessel vessel in _vessels)
                vessel.RewardIssued += OnVesselRewardIssued;
        }

        public void ResetForNewRound()
        {
            UnsubscribeVessels();

            _roundVersion++;
            _completedCount = 0;
            _pendingEffects = 0;
            _isRoundFinished = false;
            _vessels = null;

            RoundReset?.Invoke();
        }

        public void RegisterEffectStarted() => _pendingEffects++;
        public void RegisterEffectFinished() => _pendingEffects--;

        public void MarkRoundFinished()
        {
            _isRoundFinished = true;
            _completedCount = 0;
        }

        private void OnDestroy() => UnsubscribeVessels();

        private void OnVesselRewardIssued(Vector3 position, int points, Color color)
        {
            if (_isRoundFinished)
                return;

            _completedCount++;

            VesselCompleted?.Invoke(
                new VesselCompletionInfo(position, points, color, _roundVersion));

            if (IsAllVesselsComplete)
                AllVesselsCompleted?.Invoke();
        }

        private void UnsubscribeVessels()
        {
            if (_vessels == null)
                return;

            foreach (Vessel vessel in _vessels)
                vessel.RewardIssued -= OnVesselRewardIssued;
        }

        private void ValidateVesselsList(IReadOnlyList<Vessel> vessels)
        {
            Guard.NotNullOrEmpty(vessels, nameof(vessels));
            Guard.IsTrue(vessels.All(v => v != null), nameof(vessels),
                "[VesselStateTracker] Vessel list contains null entries");
        }
    }
}