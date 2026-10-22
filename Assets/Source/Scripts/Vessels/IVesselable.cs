using Assets.Source.Scripts.MagicCells;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.Vessels
{
    public interface IVesselable
    {
        public event Action<Vector3> Filled;
        public event Action<Vector3, int, Color> RewardIssued;

        public int Count { get; }
        public bool IsActive { get; }
        public bool IsFilled { get; }
        public Color Color { get; }
        public Liquid Liquid { get; }

        public void TakeMagic(MagicCell cell);
    }
}
