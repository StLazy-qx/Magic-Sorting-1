using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Enums;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.Colorize
{
    public class ColorRandomizer : MonoBehaviour
    {
        private const int BeginCountColors = 3;

        private EnumColor[] _allColors = 
            (EnumColor[])Enum.GetValues(typeof(EnumColor));
        private Color[] _beginRoundColors;
        private Color[] _remainingColors;

        public IReadOnlyList<Color> BeginColors => _beginRoundColors;
        public IReadOnlyList<Color> RemainingColors => _remainingColors;

        public void CrateArrayColors(int colorsNumber)
        {
            Guard.Positive(colorsNumber, nameof(colorsNumber));

            if (colorsNumber > _allColors.Length)
                colorsNumber = _allColors.Length;

            int[] shuffledIndices = ShuffleIndices(_allColors.Length);
            int beginCount = Mathf.Min(BeginCountColors, colorsNumber);
            _beginRoundColors = new Color[beginCount];
            _remainingColors = new Color[colorsNumber - beginCount];

            FillColorsArray(_beginRoundColors, shuffledIndices, 0);
            FillColorsArray(_remainingColors, shuffledIndices, beginCount);
        }

        private void FillColorsArray(Color[] targetArray, int[] shuffledIndices, int startIndex)
        {
            for (int i = 0; i < targetArray.Length; i++)
            {
                targetArray[i] = TransformEnumToColor(_allColors[shuffledIndices[startIndex + i]]);
            }
        }

        private int[] ShuffleIndices(int length)
        {
            Guard.Positive(length, nameof(length));

            int[] indices = new int[length];

            for (int i = 0; i < length; i++)
            {
                indices[i] = i;
            }

            for (int i = length - 1; i > 0; i--)
            {
                int index = UnityEngine.Random.Range(0, i + 1);

                (indices[i], indices[index]) = (indices[index], indices[i]);
            }

            return indices;
        }

        private Color TransformEnumToColor(EnumColor randomColor)
        {
            switch (randomColor)
            {
                case EnumColor.Red:
                    return new Color(1f, 0f, 0f);

                case EnumColor.Green:
                    return new Color(0f, 1f, 0f);

                case EnumColor.Blue:
                    return new Color(0f, 0f, 1f);

                case EnumColor.Yellow:
                    return new Color(1f, 1f, 0f);

                case EnumColor.Orange:
                    return new Color(1f, 0.5f, 0f);

                case EnumColor.Purple:
                    return new Color(0.5f, 0f, 0.5f);

                case EnumColor.Pink: 
                    return new Color(1f, 0.15f, 0.55f);

                case EnumColor.Grey:
                    return new Color(0.6f, 0.6f, 0.6f);

                case EnumColor.Cyan: 
                    return new Color(0.2f, 1f, 1f);

                case EnumColor.Lavender:
                    return new Color(0.7f, 0.5f, 0.9f);

                default:
                    return new Color(1f, 1f, 1f);
            }
        }
    }
}