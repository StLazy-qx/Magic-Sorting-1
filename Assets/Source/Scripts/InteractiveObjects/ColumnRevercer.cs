using Assets.Source.Scripts.Pool;
using Assets.Source.Scripts.MagicCells;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Source.Scripts.InteractiveObjects
{
    class ColumnRevercer : MonoBehaviour
    {
        [SerializeField] private StackMagicCells _stackHandler;

        private Vector3[] _beginCellsPosition;

        private void Awake()
        {
            if (_stackHandler == null)
            {
                throw new InvalidOperationException($"{nameof(StackMagicCells)} " +
                    $"component not found on {gameObject.name}");
            }
        }

        public Stack<MagicCell> ReverseStack(IReadOnlyList<MagicCell> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            List<MagicCell> reversedCells = new List<MagicCell>(source);

            SaveBeginPositions(reversedCells);
            reversedCells.Reverse();

            for (int i = 0; i < reversedCells.Count; i++)
            {
                MagicCell cell = reversedCells[i];

                if (cell.TryGetComponent(out MagicCellMover mover))
                {
                    mover.MoveArc(_beginCellsPosition[i]);
                }
            }

            reversedCells.Reverse();

            Stack<MagicCell> newStack = new Stack<MagicCell>();

            foreach (MagicCell cell in reversedCells)
                newStack.Push(cell);

            return newStack;
        }

        private void SaveBeginPositions(List<MagicCell> cells)
        {
            _beginCellsPosition = new Vector3[cells.Count];

            for (int i = 0; i < cells.Count; i++)
                _beginCellsPosition[i] = cells[i].transform.position;
        }
    }
}