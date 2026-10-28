using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Colorize
{
    public class ColorStrategy : IColorStrategy
    {
        public string Name { get; }
        public Color Value { get; }

        public ColorStrategy(string name, Color value)
        {
            Name = name;
            Value = value;
        }
    }
}