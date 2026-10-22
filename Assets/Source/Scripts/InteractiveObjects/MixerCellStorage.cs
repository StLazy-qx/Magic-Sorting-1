using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.MagicCells;
using UnityEngine;

namespace Assets.Source.Scripts.InteractiveObjects
{
    public class MixerCellStorage
    {
        private readonly CellPlacement _cellPlacement = new CellPlacement();
        private readonly Transform _firstStoragePoint;
        private readonly Transform _mixedCellPoint;

        private MagicCell _firstCell;
        private MagicCell _mixedCell;

        public MixerCellStorage(Transform firstStoragePoint, Transform mixedCellPoint)
        {
            Guard.NotNull(firstStoragePoint, nameof(firstStoragePoint));
            Guard.NotNull(mixedCellPoint, nameof(mixedCellPoint));

            _firstStoragePoint = firstStoragePoint;
            _mixedCellPoint = mixedCellPoint;
        }

        public MagicCell FirstCell => _firstCell;
        public MagicCell MixedCell => _mixedCell;
        public bool HasFirstCell => _firstCell != null;
        public bool HasMixedCell => _mixedCell != null;

        public void SetFirstCell(MagicCell source)
        {
            _firstCell = _cellPlacement.Instantiate(source, _firstStoragePoint, isActive: true);
        }

        public MagicCell CreateMixedCell(MagicCell source, Color color)
        {
            MagicCell mixedCell = _cellPlacement.Instantiate(source, _mixedCellPoint, isActive: true);
            
            mixedCell.SetColor(color);

            _mixedCell = mixedCell;
            
            return mixedCell;
        }

        public void DestroyFirstCell()
        {
            DestroyCell(ref _firstCell);
        }

        public void DestroyMixedCell()
        {
            DestroyCell(ref _mixedCell);
        }

        public void ReleaseMixedCell()
        {
            _mixedCell = null;
        }

        public void ClearAll()
        {
            DestroyFirstCell();
            DestroyMixedCell();
        }

        private static void DestroyCell(ref MagicCell cell)
        {
            if (cell == null)
                return;

            Object.Destroy(cell.gameObject);
            
            cell = null;
        }
    }
}