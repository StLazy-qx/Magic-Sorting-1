using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.MagicCells;
using UnityEngine;

namespace Assets.Source.Scripts.InteractiveObjects
{
    public class WaitingPoint : MonoBehaviour
    {
        private const float RotationX = 90f;

        [SerializeField] private int _seatsNumber;
        [SerializeField] private Transform _storagePoint;
        [SerializeField] private MagicCellRouter _cellRouter;
        [SerializeField] private ClickModeSwitcher _clickModeSwitcher;
        [SerializeField] private DelayDispatcher _delayDispatcher;

        private MagicCell _waitingCell;

        public Vector3 CellPosition => _waitingCell.transform.position;
        public bool IsFreePlace { get; private set; }

        private void Awake()
        {
            ValidateObjects();
            Reset();
        }

        public void AcceptStorageCell(MagicCell cell)
        {
            Guard.NotNull(cell, nameof(cell));

            if (_waitingCell != null)
                return;

            IsFreePlace = false;
            Quaternion cellRotation = Quaternion.Euler(RotationX, 0f, 0f);
            _waitingCell = Instantiate(cell, 
                _storagePoint.position, cellRotation);

            _waitingCell.Interacted += OnCellClicked;

            _waitingCell.gameObject.SetActive(false);
        }

        public void ShowWaitingCell()
        {
            _waitingCell.gameObject.SetActive(true);
        }

        public void ShowWaitingCellWithDelay()
        {
            if (_delayDispatcher != null)
                _delayDispatcher.ExecuteAfterDelay(ShowWaitingCell);
            else
                ShowWaitingCell();
        }

        public void Reset()
        {
            IsFreePlace = true;

            if (_waitingCell != null)
            {
                Destroy(_waitingCell.gameObject);

                _waitingCell = null;
            }
        }

        private void OnCellClicked()
        {
            if (_clickModeSwitcher.CurrentMode == ClickImpactMode.ModeDistribution)
            {
                MagicCell waitCell = _waitingCell;

                if (_cellRouter.IsCheckCellColor(waitCell.Color) == false)
                    return;

                _cellRouter.DeliverMagicCell(waitCell);
                _waitingCell.Disable();

                _waitingCell.Interacted -= OnCellClicked;

                Reset();
            }
        }

        private void ValidateObjects()
        {
            Guard.Positive(_seatsNumber, nameof(_seatsNumber));
            Guard.NotNull(_storagePoint, nameof(_storagePoint));
            Guard.NotNull(_cellRouter, nameof(_cellRouter));
            Guard.NotNull(_delayDispatcher, nameof(_delayDispatcher));
        }
    }
}