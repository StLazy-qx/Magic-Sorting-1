using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.MagicCells;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.InteractiveObjects
{
    public class MagicMixer : MonoBehaviour
    {
        private const float RotationX = 90f;

        [SerializeField] private Transform _firstStoragePoint;
        [SerializeField] private Transform _secondStoragePoint;
        [SerializeField] private MagicCellsFactory _magicCellsFactory;

        private ColorRecipes _colorRecipes;
        private ColorConverter _colorConverter;

        private MagicCell _firstCell;
        private MagicCell _secondCell;

        public event Action<MagicCell> CellMixed;

        public bool IsFreePlace { get; private set; }

        private void Awake()
        {
            ValidateObjects();
            Reset();
        }

        public void Initialize(ColorRecipes colorRecipes, ColorConverter colorConverter)
        {
            Guard.NotNull(colorRecipes, nameof(colorRecipes));
            Guard.NotNull(colorConverter, nameof(colorConverter));

            _colorRecipes = colorRecipes;
            _colorConverter = colorConverter;
        }

        public void AcceptCell(MagicCell cell)
        {
            Guard.NotNull(cell, nameof(cell));

            if (_firstCell == null)
            {
                AcceptFirstCell(cell);

                return;
            }

            if (_secondCell == null)
                AcceptSecondCell(cell);
        }

        public void Reset()
        {
            IsFreePlace = true;

            DestroyCell(ref _firstCell);
            DestroyCell(ref _secondCell);
        }

        private void AcceptFirstCell(MagicCell cell)
        {
            IsFreePlace = false;
            _firstCell = CreateStoredCell(cell, _firstStoragePoint);
        }

        private void AcceptSecondCell(MagicCell cell)
        {
            _secondCell = CreateStoredCell(cell, _secondStoragePoint);
            MixCells();
        }

        private MagicCell CreateStoredCell(MagicCell sourceCell, Transform storagePoint)
        {
            Quaternion cellRotation = Quaternion.Euler(RotationX, 0f, 0f);
            MagicCell storedCell = Instantiate(sourceCell, storagePoint.position, cellRotation);
            storedCell.gameObject.SetActive(false);

            return storedCell;
        }

        private void MixCells()
        {
            //EnumColor firstColor = _firstCell.EnumColor;
            //EnumColor secondColor = _secondCell.EnumColor;

            //if (_colorRecipes.TryGetResult(firstColor, secondColor, out EnumColor resultColor) == false)
            //{
            //    Reset();

            //    return;
            //}

            //Color resultUnityColor = _colorConverter.TransformEnumToColor(resultColor);
            //MagicCell mixedCell = _magicCellsFactory.CreateCell(
            //    _secondStoragePoint, Vector3.zero, resultUnityColor);

            //Reset();

            //CellMixed?.Invoke(mixedCell);
        }

        private void DestroyCell(ref MagicCell cell)
        {
            if (cell == null)
                return;

            Destroy(cell.gameObject);
            cell = null;
        }

        private void ValidateObjects()
        {
            Guard.NotNull(_firstStoragePoint, nameof(_firstStoragePoint));
            Guard.NotNull(_secondStoragePoint, nameof(_secondStoragePoint));
            Guard.NotNull(_magicCellsFactory, nameof(_magicCellsFactory));
        }
    }
}