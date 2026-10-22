using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.MagicCells;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.InteractiveObjects
{
    public class MagicMixer : MonoBehaviour
    {
        [SerializeField] private Transform _firstStoragePoint;
        [SerializeField] private Transform _mixedCellPoint;
        [SerializeField] private MagicCellsFactory _magicCellsFactory;
        [SerializeField] private MagicCellRouter _cellRouter;
        [SerializeField] private ClickModeSwitcher _clickModeSwitcher;

        private MixerState _state;
        private MixerCellStorage _storage;
        private CellClickDeliveryHandler _deliveryHandler;

        public event Action<MagicCell> CellMixed;
        public event Action<Color> ColorsMixed;
        public event Action<MagicCell, MagicCell> MixFailed;

        public bool CanAcceptCell => _state?.CanAcceptCell ?? false;
        public bool HasMixedCell => _storage?.HasMixedCell ?? false;

        public bool IsInitialized { get; private set; }

        public void Initialize(
            ColorRecipes colorRecipes,
            ColorConverter colorConverter)
        {
            Guard.NotNull(colorRecipes, nameof(colorRecipes));
            Guard.NotNull(colorConverter, nameof(colorConverter));

            ValidateObjects();

            _state = new MixerState(colorRecipes, colorConverter);
            _storage = new MixerCellStorage(_firstStoragePoint, _mixedCellPoint);
            _deliveryHandler = new CellClickDeliveryHandler(
                _cellRouter,
                _clickModeSwitcher,
                allowMixerDelivery: false);
            _deliveryHandler.Delivered += OnMixedCellDelivered;

            Reset();

            IsInitialized = true;
        }

        public bool CanAcceptCellColor(Color color)
        {
            if (IsInitialized == false)
                return false;

            return _state.CanAcceptCellColor(color);
        }

        public void AcceptCell(MagicCell cell)
        {
            Guard.NotNull(cell, nameof(cell));

            if (_state.CanAcceptCell == false)
                return;

            if (_storage.HasFirstCell == false)
            {
                AcceptFirstCell(cell);
                
                return;
            }

            AcceptSecondCell(cell);
        }

        public void Reset()
        {
            _state?.Reset();
            _storage?.ClearAll();
            _deliveryHandler?.Detach();
        }

        private void AcceptFirstCell(MagicCell cell)
        {
            _storage.SetFirstCell(cell);
            _state.AcceptFirst(cell.Color);
        }

        private void AcceptSecondCell(MagicCell cell)
        {
            MagicCell firstCell = _storage.FirstCell;

            if (_state.TryMix(cell.Color, out Color resultColor) == false)
            {
                MixFailed?.Invoke(firstCell, cell);
                
                return;
            }

            MagicCell mixedCell = _storage.CreateMixedCell(firstCell, resultColor);
            
            _storage.DestroyFirstCell();
            _deliveryHandler.Attach(mixedCell);
            CellMixed?.Invoke(mixedCell);
            ColorsMixed?.Invoke(resultColor);
        }

        private void OnMixedCellDelivered(MagicCell cell)
        {
            if (_storage.MixedCell != cell)
                return;

            _deliveryHandler.Detach();
            _storage.ReleaseMixedCell();
            _state.Reset();
        }

        private void ValidateObjects()
        {
            Guard.NotNull(_firstStoragePoint, nameof(_firstStoragePoint));
            Guard.NotNull(_mixedCellPoint, nameof(_mixedCellPoint));
            Guard.NotNull(_magicCellsFactory, nameof(_magicCellsFactory));
            Guard.NotNull(_cellRouter, nameof(_cellRouter));
            Guard.NotNull(_clickModeSwitcher, nameof(_clickModeSwitcher));
        }
    }
}