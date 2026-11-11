using UnityEngine;

namespace Assets.Source.Scripts.Vessels
{
    public readonly struct VesselCompletionInfo
    {
        public readonly Vector3 Position;
        public readonly int Points;
        public readonly Color Color;
        public readonly int RoundVersion;

        public VesselCompletionInfo(Vector3 position, int points, Color color, int roundVersion)
        {
            Position = position;
            Points = points;
            Color = color;
            RoundVersion = roundVersion;
        }
    }
}