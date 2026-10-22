using UnityEngine;

namespace Assets.Source.Scripts.Vessels
{
    public static class VesselReward
    {
        public static int Calculate(int points, int maxSize, int currentVolume)
        {
            if (maxSize <= 0 || points <= 0)
                return 0;

            int reward = points * currentVolume / maxSize;

            return Mathf.Clamp(reward, 0, points);
        }
    }
}