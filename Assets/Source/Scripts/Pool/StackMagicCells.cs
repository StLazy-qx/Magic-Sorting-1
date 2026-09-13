using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.MagicCells;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Pool
{
    public class StackMagicCells : MonoBehaviour
    {
        [SerializeField] private GameSessionHandler _gameHandler;
        [SerializeField] private ColumnRevercer _columnRevercer;

        private MagicCellsFactory _factory;
        private MagicCellRouter _cellRouter;
        private ShuffledColorDistributor _colorSource;
        private EntryListColorPool _listColorPool;
        private ClickModeSwitcher _clickImpactHandler;
        private Transform _parent;
        private int _maxVolumeCells;
        private float _prefabHeight;
        private bool _isPaused;
        private Stack<MagicCell> _cellsStack = new();

        private void OnDisable()
        {
            _gameHandler.PauseStateChanged -= OnGamePause;
        }

        public void Initialize(
            MagicCellsFactory factory,
            MagicCellRouter cellRouter,
            ShuffledColorDistributor colorSource,
            EntryListColorPool listColorPool,
            ClickModeSwitcher clickHandler,
            Transform parent,
            int maxVolumeCells,
            float prefabHeight)
        {
            ValidateArguments(factory, cellRouter, colorSource, clickHandler, parent);
            ValidateValues(maxVolumeCells, prefabHeight);
            ClearStack();

            _factory = factory;
            _cellRouter = cellRouter;
            _colorSource = colorSource;
            _listColorPool = listColorPool;
            _clickImpactHandler = clickHandler;
            _parent = parent;
            _maxVolumeCells = maxVolumeCells;
            _prefabHeight = prefabHeight;

            _gameHandler.PauseStateChanged += OnGamePause;
        }

        public void CreateCell(Color color)
        {
            if (CanAddCell() == false)
                return;

            float currentY = _cellsStack.Count * _prefabHeight;

            MagicCell cell = _factory.CreateCell(
                parent: _parent,
                localPosition: new Vector3(0, currentY, 0),
                color: color);

            cell.Interacted += OnCellClicked;

            _cellsStack.Push(cell);
        }

        public bool CanAddCell()
        {
            return _cellsStack.Count < _maxVolumeCells;
        }

        public MagicCell TryGetCellByColor(Color color)
        {
            MagicCell topCell = GetUpperCell();

            if (topCell == null)
                return null;

            return topCell.Color == color ? topCell : null;
        }

        public bool CheckLastCells(Color color)
        {
            if (_cellsStack.Count < 2)
                return false;

            MagicCell[] cells = _cellsStack.ToArray();
            int cellNumber = cells.Length - 1;

            return cells[cellNumber].Color == color;
        }

        public MagicCell GetBottomCell()
        {
            if (_cellsStack.Count == 0)
                return null;

            MagicCell bottomCell = null;

            foreach (MagicCell cell in _cellsStack)
                bottomCell = cell;

            return bottomCell;
        }

        public MagicCell GetUpperCell()
        {
            return _cellsStack.TryPeek(out MagicCell cell) ? 
                cell : null;
        }

        public bool CanReverseColumn()
        {
            return HasMoreOneCell() && AreDifferentColor();
        }

        private void OnGamePause(bool isPaused)
        {
            _isPaused = isPaused;
        }

        private void ClearStack()
        {
            while (_cellsStack.Count > 0)
            {
                MagicCell cell = _cellsStack.Pop();
                cell.Interacted -= OnCellClicked;

                if (cell != null)
                    Destroy(cell.gameObject);
            }
        }

        private void OnCellClicked()
        {
            if (_isPaused)
                return;

            if (_clickImpactHandler.CurrentMode == ClickImpactMode.ModeDistribution)
            {
                if (_cellsStack.Count == 0)
                    return;

                if (_cellsStack.TryPop(out MagicCell cell) == false)
                    return;

                if (_cellRouter.IsCheckCellColor(cell.Color) == false)
                {
                    _cellsStack.Push(cell);

                    return;
                }

                _cellRouter.DeliverMagicCell(cell);
                cell.Disable();
            }
            else if (
                _clickImpactHandler.CurrentMode == 
                ClickImpactMode.ModeReverse && 
                CanReverseColumn())
            {
                _cellsStack = _columnRevercer.ReverseStack(_cellsStack.ToArray());

                _clickImpactHandler.Reverse();
                _clickImpactHandler.OnToggleMode();
            }
        }

        private bool HasMoreOneCell()
        {
            return _cellsStack.Count > 1;
        }

        private bool AreDifferentColor()
        {
            if (_cellsStack.Count == 0)
                return false;

            Color topColor = _cellsStack.Peek().Color;

            foreach (MagicCell cell in _cellsStack)
            {
                if (cell.Color != topColor)
                    return true;
            }

            return false;
        }

        private void ValidateArguments(
        MagicCellsFactory factory,
        MagicCellRouter cellRouter,
        ShuffledColorDistributor colorSource,
        ClickModeSwitcher clickImpactHandler,
        Transform parent)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (cellRouter == null)
                throw new ArgumentNullException(nameof(cellRouter));

            if (colorSource == null)
                throw new ArgumentNullException(nameof(colorSource));

            if (clickImpactHandler == null)
                throw new ArgumentNullException(nameof(clickImpactHandler));

            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
        }

        private void ValidateValues(int countCells, float prefabHeight)
        {
            if (countCells <= 0)
                throw new ArgumentException(nameof(countCells));

            if (prefabHeight <= 0)
                throw new ArgumentException(nameof(prefabHeight));
        }
    }
}