using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.UI.GamePanel;
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
        [SerializeField] private RoundSequenceTracker _roundTracker;

        private Wallet _wallet;
        private Panel _currentPanel;
        private int _roundScore;

        public event Action RoundOvering;
        public event Action VictoryAudioClipEnabled;
        public event Action<RoundRating> ResultSummarizing;

        public bool IsInitialized { get; private set; }

        [Inject]
        public void Construct(Wallet wallet)
        {
            _wallet = wallet;
        }

        private void OnEnable() => SetSubscriptions(true);

        private void OnDisable() => SetSubscriptions(false);

        public void Initialize()
        {
            if (IsInitialized)
                return;

            ValidateInitializeArguments();
            IsInitialized = true;
        }

        public void SetFinalPanels(Panel endRoundPanel, Panel endModePanel)
        {
            Guard.NotNull(endModePanel, nameof(endModePanel));

            SetCurrentPanel(endRoundPanel);
            _finalGame.SetSessionEndPanels(endRoundPanel, endModePanel);
        }

        public void SetTutorialEndRoundPanel(Panel endRoundPanel)
        {
            SetCurrentPanel(endRoundPanel);
            _finalGame.SetTutorialPanel(endRoundPanel);
        }

        public void SetVesselsList(IReadOnlyList<Vessel> vessels)
        {
            _effecter.Initialize(vessels.Count);
            _tracker.SetVesselsList(vessels);
        }

        public bool IsAllVesselsComplete()
            => _tracker.IsAllVesselsComplete;

        private void SetSubscriptions(bool subscribe)
        {
            if (subscribe)
            {
                _tracker.VesselCompleted += OnVesselCompleted;
                _tracker.AllVesselsCompleted += OnAllVesselsCompleted;
                _tracker.RoundReset += OnRoundReset;

                _roundTracker.RoundOpened += OnRoundOpened;
                _roundTracker.RoundCompleted += OnRoundCompleted;
                _roundTracker.ModeCompleted += OnModeCompleted;
            }
            else
            {
                _tracker.VesselCompleted -= OnVesselCompleted;
                _tracker.AllVesselsCompleted -= OnAllVesselsCompleted;
                _tracker.RoundReset -= OnRoundReset;

                _roundTracker.RoundOpened -= OnRoundOpened;
                _roundTracker.RoundCompleted -= OnRoundCompleted;
                _roundTracker.ModeCompleted -= OnModeCompleted;
            }
        }

        private void SetCurrentPanel(Panel panel)
        {
            Guard.NotNull(panel, nameof(panel));
            _currentPanel = panel;
        }

        private void OnRoundOpened()
        {
            _roundScore = 0;
            _tracker.ResetForNewRound();
            _effecter.ResetPool();
        }

        private void OnRoundReset()
            => StopAllCoroutines();

        private void OnAllVesselsCompleted()
            => VictoryAudioClipEnabled?.Invoke();

        private void OnVesselCompleted(VesselCompletionInfo info)
        {
            _tracker.RegisterEffectStarted();
            _wallet.AddPoints(info.Points);
            _roundScore += info.Points;
            StartCoroutine(PerformEffect(info));
        }

        private IEnumerator PerformEffect(VesselCompletionInfo info)
        {
            RoundOvering?.Invoke();

            yield return _effecter.PlayEffect(info.Position, info.Color);

            if (info.RoundVersion != _tracker.RoundVersion)
                yield break;

            _tracker.RegisterEffectFinished();

            if (_tracker.IsReadyToFinish == false)
                yield break;

            _tracker.MarkRoundFinished();
            CompleteRoundWithRating();
        }

        private void OnRoundCompleted(GameMode mode, int roundNumber)
        {
            if (_roundTracker.IsLastRound)
                return;

            _finalGame.ShowEndRoundPanel();
        }

        private void OnModeCompleted(GameMode mode)
        {
            if (mode == GameMode.Tutorial)
            {
                _finalGame.ShowTutorialPanel();
                return;
            }

            _finalGame.ShowEndModePanel();
            CompleteRoundWithRating();
        }

        private void CompleteRoundWithRating()
        {
            RoundRating rating = RoundRankCalculator.Calculate(
                _roundScore, _tracker.TotalCount);

            _wallet.ConfirmPoints();
            _currentPanel.Close();
            ResultSummarizing?.Invoke(rating);
            _roundTracker.CompleteRound(rating);
        }

        private void ValidateInitializeArguments()
        {
            Guard.NotNull(_tracker, nameof(_tracker));
            Guard.NotNull(_effecter, nameof(_effecter));
            Guard.NotNull(_finalGame, nameof(_finalGame));
            Guard.NotNull(_roundTracker, nameof(_roundTracker));
            Guard.NotNull(_currentPanel, nameof(_currentPanel));
        }
    }
}