using Assets.Source.Scripts.Enums;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.GameDifficulty
{
    [CreateAssetMenu(menuName = "Config/Difficulty Setting", 
        fileName = "Difficulty Setting", order = 51)]

    public class DifficultySettings : ScriptableObject
    {
        public DifficultyLevel Level;
        [Header("Columns settings")]
        public int ColumnsCount;
        public int MaxCellsPerColumn;
        [Header("Vessels settings")]
        public int VesselsCount;
        public int ColorsCount;

        private void OnValidate()
        {
            ValidateValue(ColumnsCount, nameof(ColumnsCount));
            ValidateValue(MaxCellsPerColumn, nameof(MaxCellsPerColumn));
            ValidateValue(VesselsCount, nameof(VesselsCount));
            ValidateValue(ColorsCount, nameof(ColorsCount));
        }

        private void ValidateValue(int value, string paramName)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName,
                    $"{paramName} должен быть > 0");
            }
        }
    }
}