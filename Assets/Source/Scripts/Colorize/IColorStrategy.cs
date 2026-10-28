using System;
using Assets.Source.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Scripts.Colorize
{
    public interface IColorStrategy
    {
        string Name { get; }
        Color Value { get; }
    }
}