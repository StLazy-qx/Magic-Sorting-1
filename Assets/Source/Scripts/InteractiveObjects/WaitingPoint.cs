using Assets.Source.Scripts.ActionsHandlers;
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

        private CellClickDeliveryHandler _deliveryHandler;
        private MagicCell _waitingCell;

        public bool IsFreePlace { get; private set; }

        private void Awake()
        {
            ValidateObjects();

            _deliveryHandler = new CellClickDeliveryHandler(
                _cellRouter,
                _clickModeSwitcher,
                allowMixerDelivery: true);
            _deliveryHandler.Delivered += OnWaitingCellDelivered;

            Reset();
        }

        public void AcceptStorageCell(MagicCell cell)
        {
            Guard.NotNull(cell, nameof(cell));

            if (_waitingCell != null)
                return;

            IsFreePlace = false;

            _waitingCell = _cellPlacement.Instantiate(cell, _storagePoint);
            _deliveryHandler.Attach(_waitingCell);

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
                _deliveryHandler.Detach();
                Destroy(_waitingCell.gameObject);
                _waitingCell = null;
            }
        }

        private void OnWaitingCellDelivered(MagicCell cell)
        {
            if (_waitingCell != cell)
                return;

            _waitingCell = null;
            IsFreePlace = true;
        }

        private void ValidateObjects()
        {
            Guard.NotNull(_storagePoint, nameof(_storagePoint));
            Guard.NotNull(_cellRouter, nameof(_cellRouter));
            Guard.NotNull(_delayDispatcher, nameof(_delayDispatcher));
        }
    }
}