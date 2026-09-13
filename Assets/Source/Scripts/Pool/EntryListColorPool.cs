using System.Collections.Generic;
using Assets.Source.Scripts.Colorize;
using UnityEngine;

namespace Assets.Source.Scripts.Pool
{
    public class EntryListColorPool : MonoBehaviour
    {
        private readonly List<ColorEntry> _beginingPoolColors = new List<ColorEntry>();
        private readonly List<ColorEntry> _remainingPoolColors = new List<ColorEntry>();

        public void AddBeginingColors(ColorEntry color)
        {
            if (color == null)
                return;

            _beginingPoolColors.Add(color);
        }

        public void AddRemainingColors(ColorEntry color)
        {
            if (color == null)
                return;

            _remainingPoolColors.Add(color);
        }

        public ColorEntry Get()
        {
            ColorEntry entry = GetFromList(_remainingPoolColors);

            if (entry != null)
                return entry;

            return GetFromList(_beginingPoolColors);
        }

        private ColorEntry GetFromList(List<ColorEntry> pool)
        {
            if (pool.Count == 0)
                return null;

            int startIndex = Random.Range(0, pool.Count);

            for (int i = 0; i < pool.Count; i++)
            {
                int index = (startIndex + i) % pool.Count;
                ColorEntry entry = pool[index];

                if (entry.IsEmpty() == false)
                    return entry;
            }

            return null;
        }

        public void Clear()
        {
            _beginingPoolColors.Clear();
            _remainingPoolColors.Clear();
        }
    }
}
