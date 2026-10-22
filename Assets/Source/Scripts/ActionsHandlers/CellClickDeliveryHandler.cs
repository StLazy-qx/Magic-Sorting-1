using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.MagicCells;
using System;

namespace Assets.Source.Scripts.InteractiveObjects
{
    /// <summary>
    /// Инкапсулирует общую логику обработки клика по ячейке:
    /// проверка режима клика, проверка возможности доставки, доставка и отписка.
    /// </summary>
    public class CellClickDeliveryHandler
    {
        private readonly MagicCellRouter _cellRouter;
        private readonly ClickModeSwitcher _clickModeSwitcher;
        private readonly bool _allowMixerDelivery;

        private MagicCell _cell;

        public CellClickDeliveryHandler(
            MagicCellRouter cellRouter,
            ClickModeSwitcher clickModeSwitcher,
            bool allowMixerDelivery)
        {
            Guard.NotNull(cellRouter, nameof(cellRouter));
            Guard.NotNull(clickModeSwitcher, nameof(clickModeSwitcher));

            _cellRouter = cellRouter;
            _clickModeSwitcher = clickModeSwitcher;
            _allowMixerDelivery = allowMixerDelivery;
        }

        public event Action<MagicCell> Delivered;
        public event Action<MagicCell> MixerDeliveryFailed;

        public MagicCell Cell => _cell;
        public bool HasCell => _cell != null;

        public void Attach(MagicCell cell)
        {
            Guard.NotNull(cell, nameof(cell));

            if (_cell == cell)
                return;

            Detach();

            _cell = cell;
            _cell.Interacted += OnCellClicked;
        }

        public void Detach()
        {
            if (_cell == null)
                return;

            _cell.Interacted -= OnCellClicked;
            _cell = null;
        }

        private void OnCellClicked()
        {
            if (_cell == null)
                return;

            switch (_clickModeSwitcher.CurrentMode)
            {
                case ClickImpactMode.ModeDistribution:
                    TryDeliverToVessel();
                    break;

                case ClickImpactMode.ModeColorMixing:
                    if (_allowMixerDelivery)
                        TryDeliverToMixer();
                    break;
            }
        }

        private void TryDeliverToVessel()
        {
            MagicCell cell = _cell;

            if (_cellRouter.IsCheckCellColor(cell.Color) == false)
                return;

            Detach();

            _cellRouter.DeliverMagicCell(cell);
            cell.Disable();

            Delivered?.Invoke(cell);
        }

        private void TryDeliverToMixer()
        {
            MagicCell cell = _cell;

            if (_cellRouter.CanAcceptCellMixer(cell) == false)
                return;

            Detach();

            if (_cellRouter.DeliverMagicCellMixer(cell) == false)
            {
                MixerDeliveryFailed?.Invoke(cell);
                return;
            }

            cell.Disable();

            Delivered?.Invoke(cell);
        }
    }
}