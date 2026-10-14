using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.EntryPoint;
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
        [SerializeField] private Transform _secondStoragePoint;
        [SerializeField] private MagicCellsFactory _magicCellsFactory;
        [SerializeField] private MagicCellRouter _cellRouter;
        [SerializeField] private ClickModeSwitcher _clickModeSwitcher;

        private readonly CellPlacement _cellPlacement = new CellPlacement();

        private ColorRecipes _colorRecipes;
        private ColorConverter _colorConverter;

        private MagicCell _firstCell;
        private MagicCell _mixedCell;

        public event Action<MagicCell> CellMixed;
        public event Action<Color> ColorsMixed;

        public bool CanAcceptCell => _mixedCell == null;
        public bool HasMixedCell => _mixedCell != null;

        public bool IsInitialized { get; private set; }

        //инициализировать
        public void Initialize(
            ColorRecipes colorRecipes,
            ColorConverter colorConverter)
        {
            Guard.NotNull(colorRecipes, nameof(colorRecipes));
            Guard.NotNull(colorConverter, nameof(colorConverter));

            _colorRecipes = colorRecipes;
            _colorConverter = colorConverter;

            ValidateObjects();
            Reset();

            IsInitialized = true;
        }

        public void AcceptCell(MagicCell cell)
        {
            Guard.NotNull(cell, nameof(cell));

            if (CanAcceptCell == false)
                return;

            if (_firstCell == null)
            {
                AcceptFirstCell(cell);
                return;
            }

            AcceptSecondCell(cell);
        }

        public void Reset()
        {
            DestroyCell(ref _firstCell);
            DestroyCell(ref _mixedCell);
        }

        private void AcceptFirstCell(MagicCell cell)
        {
            _firstCell = CreateStoredCell(cell, _firstStoragePoint, isActive: true);
        }

        private void AcceptSecondCell(MagicCell cell)
        {
            EnumColor firstColor = _colorConverter.TransformColorToEnum(_firstCell.Color);
            EnumColor secondColor = _colorConverter.TransformColorToEnum(cell.Color);

            if (_colorRecipes.TryGetResult(
                    firstColor, secondColor, out EnumColor resultColor) == false)
            {
                Reset();
                return;
            }

            Color resultUnityColor = _colorConverter.TransformEnumToColor(resultColor);

            _firstCell.SetColor(resultUnityColor);

            _mixedCell = _firstCell;
            _firstCell = null;
            _mixedCell.Interacted += OnMixedCellClicked;

            CellMixed?.Invoke(_mixedCell);
            ColorsMixed?.Invoke(resultUnityColor);
        }

        private MagicCell CreateStoredCell(MagicCell sourceCell,
            Transform storagePoint, bool isActive)
        {
            return _cellPlacement.Instantiate(sourceCell, storagePoint, isActive);
        }

        private void OnMixedCellClicked()
        {
            if (_clickModeSwitcher.CurrentMode != ClickImpactMode.ModeDistribution)
                return;

            if (_cellRouter.IsCheckCellColor(_mixedCell.Color) == false)
                return;

            MagicCell cellToDeliver = _mixedCell;
            _mixedCell.Interacted -= OnMixedCellClicked;
            _mixedCell = null;

            _cellRouter.DeliverMagicCell(cellToDeliver);
            cellToDeliver.Disable();
        }

        private void DestroyCell(ref MagicCell cell)
        {
            if (cell == null)
                return;

            cell.Interacted -= OnMixedCellClicked;

            Destroy(cell.gameObject);

            cell = null;
        }

        private void ValidateObjects()
        {
            Guard.NotNull(_firstStoragePoint, nameof(_firstStoragePoint));
            Guard.NotNull(_secondStoragePoint, nameof(_secondStoragePoint));
            Guard.NotNull(_magicCellsFactory, nameof(_magicCellsFactory));
            Guard.NotNull(_cellRouter, nameof(_cellRouter));
            Guard.NotNull(_clickModeSwitcher, nameof(_clickModeSwitcher));
        }


    }
}