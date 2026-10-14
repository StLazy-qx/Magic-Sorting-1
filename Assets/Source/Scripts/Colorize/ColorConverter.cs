using System;
using Assets.Source.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Scripts.Colorize
{
    public class ColorConverter
    {
        public Color TransformEnumToColor(EnumColor enumColor)
        {
            switch (enumColor)
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

                case EnumColor.Black:
                    return new Color(0f, 0f, 0f);

                case EnumColor.Magenta:
                    return new Color(1f, 0f, 1f);

                case EnumColor.White:
                    return new Color(1f, 1f, 1f);

                default:
                    return new Color(1f, 0.5f, 1f);
            }
        }
        
		public EnumColor TransformColorToEnum(Color color)
        {
            EnumColor closestEnumColor = default;
            float minimalDistance = float.MaxValue;

            foreach (EnumColor enumColor in Enum.GetValues(typeof(EnumColor)))
            {
                Color candidateColor = TransformEnumToColor(enumColor);

                float redDifference = candidateColor.r - color.r;
                float greenDifference = candidateColor.g - color.g;
                float blueDifference = candidateColor.b - color.b;
                float distance = redDifference * redDifference
                                 + greenDifference * greenDifference
                                 + blueDifference * blueDifference;

                if (distance < minimalDistance)
                {
                    minimalDistance = distance;
                    closestEnumColor = enumColor;
                }
            }

            return closestEnumColor;
        }
    }
}