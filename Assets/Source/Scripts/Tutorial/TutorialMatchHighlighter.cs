using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.Pool;
using Assets.Source.Scripts.UI.Buttons;
using Assets.Source.Scripts.Vessels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Tutorial
{
    public class TutorialMatchHighlighter : MonoBehaviour
    {
        [SerializeField] private GameSessionHandler _gameHandler;
        [SerializeField] private AnimationParticle _animationParticle;
        [SerializeField] private MagicCellRouter _cellRouter;
        [SerializeField] private MagicColumnPool _magicColumnPool;
        [SerializeField] private VesselPool _vesselPool;
        [SerializeField] private VesselFactory _vesselFactory;
        [SerializeField] private WaitingPoint _waitingPoint;
        [SerializeField] private ClickModeSwitcher _modeSwitcher;
        [SerializeField] private Transform[] _columns;

        private float _beginsearchInterval = 1.5f;
        private float _searchInterval = 8f;
        private ReverseButton _reverseButton;
        private AsyncTimer _timer;
        private List<MagicColumn> _currentColumns;
        private List<Vessel> _currentVessels;

        private void Awake()
        {
            _timer = new AsyncTimer();
            _currentVessels = new List<Vessel>();
            _currentColumns = new List<MagicColumn>();

            ValidateDependencies();
        }

        private void Start()
        {
            _timer.StartTimer(_beginsearchInterval, PerformAnalysis);
        }

        private void OnEnable()
        {
            _gameHandler.GameLaunching += OnStartInitialSearch;
            _cellRouter.CellDeparturing += OnStartSearchLoop;
            _modeSwitcher.RewardedEnded += OnStartSearchLoopAfterRewarded;
        }

        private void OnDisable()
        {
            _gameHandler.GameLaunching -= OnStartInitialSearch;
            _cellRouter.CellDeparturing -= OnStartSearchLoop;
            _modeSwitcher.RewardedEnded -= OnStartSearchLoopAfterRewarded;

            if (_reverseButton != null)
                _modeSwitcher.ReverseButtonActivating -= OnReverseButtonActivated;

            _timer.StopTimer();
        }

        public void SetButtonRewarded(
            IconRewardedAdvertisement rewardedButton,
            ReverseButton reverseButton)
        {
            Guard.NotNull(rewardedButton, nameof(rewardedButton));
            Guard.NotNull(reverseButton, nameof(reverseButton));

            _reverseButton = reverseButton;
            _modeSwitcher.ReverseButtonActivating += OnReverseButtonActivated;
        }

        public void LoadCurrentColumns()
        {
            _currentColumns.Clear();
            _currentColumns.AddRange(_magicColumnPool.GetActiveObjects());
        }

        public void LoadCurrentVessels()
        {
            _currentVessels.Clear();

            foreach (var vessel in _vesselFactory.Objects)
            {
                if (vessel.IsActive)
                    _currentVessels.Add(vessel);
            }
        }

        private void OnStartSearchLoopAfterRewarded()
        {
            _animationParticle.Stop();
            UpdateReverseButtonAvailability();
            _timer.StartTimer(0, PerformAnalysis);
        }

        private void OnStartSearchLoop()
        {
            _animationParticle.Stop();
            UpdateReverseButtonAvailability();
            _timer.StartTimer(_searchInterval, PerformAnalysis);
        }

        private void OnStartInitialSearch()
        {
            _reverseButton.UIButton.interactable = true;

            _animationParticle.Stop();
            _timer.StartTimer(_beginsearchInterval, PerformAnalysis);
        }

        private void OnReverseButtonActivated()
        {
            bool anyCanReverse = false;

            LoadCurrentColumns();
            LoadCurrentVessels();

            foreach (MagicColumn column in _currentColumns)
            {
                StackMagicCells stack = column.GetComponent<StackMagicCells>();

                if (stack.CanReverseColumn())
                {
                    anyCanReverse = true;
                    break;
                }
            }

            if (anyCanReverse == false)
            {
                Button reverseButton = GetComponentButton(_reverseButton);
                if (reverseButton != null)
                    _animationParticle.Play(reverseButton);
                return;
            }

            MagicColumn targetColumn = FindReverseColumn();

            if (targetColumn == null)
                return;

            StackMagicCells stackForTarget = targetColumn.GetComponent<StackMagicCells>();
            MagicCell cell = stackForTarget.GetUpperCell();

            if (cell != null)
                _animationParticle.Play(cell.transform.position);
        }

        private MagicColumn FindReverseColumn()
        {
            int thirdIndex = 3;
            int vesselsToCheck = Mathf.Min(_currentVessels.Count, thirdIndex);

            foreach (MagicColumn column in _currentColumns)
            {
                StackMagicCells stack = column.GetComponent<StackMagicCells>();

                for (int i = 0; i < vesselsToCheck; i++)
                {
                    if (stack.CheckLastCells(_currentVessels[i].Color))
                        return column;
                }
            }

            return null;
        }

        private void PerformAnalysis()
        {
            LoadCurrentColumns();
            LoadCurrentVessels();
            FindMatch();

            _timer.StartTimer(_searchInterval, PerformAnalysis);
        }

        private void FindMatch()
        {
            if (TryFindTopMatch())
                return;

            MagicCell waitingPointCandidate = FindBottomMatchForAnyVessel();

            ResolveNoMatchCases(waitingPointCandidate);
        }

        private bool TryFindTopMatch()
        {
            foreach (Vessel vessel in _currentVessels)
            {
                Color color = vessel.Color;

                foreach (MagicColumn column in _currentColumns)
                {
                    StackMagicCells stack = column.GetComponent<StackMagicCells>();
                    MagicCell topCell = stack.TryGetCellByColor(color);

                    if (topCell != null)
                    {
                        PlayAnimationCell(topCell);

                        return true;
                    }
                }
            }

            return false;
        }

        private MagicCell FindBottomMatchForAnyVessel()
        {
            foreach (Vessel vessel in _currentVessels)
            {
                Color color = vessel.Color;

                foreach (MagicColumn column in _currentColumns)
                {
                    StackMagicCells stack = column.GetComponent<StackMagicCells>();
                    MagicCell bottomCell = stack.GetBottomCell();

                    if (bottomCell != null && bottomCell.Color == color)
                        return bottomCell;
                }
            }

            return null;
        }

        private void ResolveNoMatchCases(MagicCell waitingPointCandidate)
        {
            if (_waitingPoint.IsFreePlace)
            {
                if (waitingPointCandidate != null)
                    _animationParticle.Play(waitingPointCandidate.transform.position);

                return;
            }

            if (_modeSwitcher.CurrentMode == _modeSwitcher.CurrentMode)
                return;


            Button reverseButton = GetComponentButton(_reverseButton);

            _animationParticle.Play(reverseButton);
        }

        private void UpdateReverseButtonAvailability()
        {
            if (_reverseButton == null)
                return;

            Button button = GetComponentButton(_reverseButton);

            if (AreAllColumnsEmpty())
            {
                button.interactable = false;

                return;
            }

            button.interactable = true;
        }

        private bool AreAllColumnsEmpty()
        {
            LoadCurrentColumns();

            if (_currentColumns.Count == 0)
                return true;

            foreach (MagicColumn column in _currentColumns)
            {
                StackMagicCells stack = column.GetComponent<StackMagicCells>();

                if (stack.GetUpperCell() != null)
                    return false;
            }

            return true;
        }

        private void PlayAnimationCell(MagicCell magicCell)
        {
            Guard.NotNull(magicCell, nameof(magicCell));
            _animationParticle.Play(magicCell.transform.position);
        }

        private Button GetComponentButton(Component component)
        {
            if (component == null)
                return null;

            Button button = component.GetComponent<Button>();
            Guard.NotNull(button, nameof(button));

            return button;
        }

        private void ValidateDependencies()
        {
            Guard.NotNull(_animationParticle, nameof(_animationParticle));
            Guard.NotNull(_cellRouter, nameof(_cellRouter));
            Guard.NotNull(_magicColumnPool, nameof(_magicColumnPool));
            Guard.NotNull(_vesselPool, nameof(_vesselPool));
            Guard.NotNull(_waitingPoint, nameof(_waitingPoint));
        }
    }
}
