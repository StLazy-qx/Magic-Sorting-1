using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.MagicCells;
using UnityEngine;

namespace Assets.Source.Scripts.InteractiveObjects
{
    public class WaitingPoint : MonoBehaviour
    {
        [SerializeField] private Transform _storagePoint;
        [SerializeField] private MagicCellRouter _cellRouter;
        [SerializeField] private ClickModeSwitcher _clickModeSwitcher;
        [SerializeField] private DelayDispatcher _delayDispatcher;

        private readonly CellPlacement _cellPlacement = new CellPlacement();

        private MagicCell _waitingCell;

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

            _waitingCell = _cellPlacement.Instantiate(cell, _storagePoint);
            _waitingCell.Interacted += OnCellClicked;

            _waitingCell.gameObject.SetActive(false);
        }
        
        public void ShowWaitingCell()
            => _waitingCell.gameObject.SetActive(true);

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
            if (_waitingCell == null)
                return;

            switch (_clickModeSwitcher.CurrentMode)
            {
                case ClickImpactMode.ModeDistribution:
                    TryDeliverToVessel();
                    break;

                case ClickImpactMode.ModeColorMixing:
                    TryDeliverToMixer();
                    break;
            }

            //if (_clickModeSwitcher.CurrentMode == ClickImpactMode.ModeDistribution)
            //{
            //    MagicCell waitCell = _waitingCell;

            //    if (_cellRouter.IsCheckCellColor(waitCell.Color) == false)
            //        return;

            //    _cellRouter.DeliverMagicCell(waitCell);
            //    _waitingCell.Disable();

            //    _waitingCell.Interacted -= OnCellClicked;

            //    Reset();
            //}
        }

        private void TryDeliverToVessel()
        {
            MagicCell waitCell = _waitingCell;

            if (_cellRouter.IsCheckCellColor(waitCell.Color) == false)
                return;

            ReleaseWaitingCell();

            _cellRouter.DeliverMagicCell(waitCell);
            waitCell.Disable();
        }

        private void TryDeliverToMixer()
        {
            MagicCell waitCell = _waitingCell;

            if (_cellRouter.CanAcceptCellMixer(waitCell) == false)
                return;

            ReleaseWaitingCell();

            if (_cellRouter.DeliverMagicCellMixer(waitCell) == false)
                return;

            waitCell.Disable();
        }

        private void ReleaseWaitingCell()
        {
            _waitingCell.Interacted -= OnCellClicked;
            IsFreePlace = true;
            _waitingCell = null;
        }

        private void ValidateObjects()
        {
            Guard.NotNull(_storagePoint, nameof(_storagePoint));
            Guard.NotNull(_cellRouter, nameof(_cellRouter));
            Guard.NotNull(_delayDispatcher, nameof(_delayDispatcher));
        }
    }
}