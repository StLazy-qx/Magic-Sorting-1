using Assets.Source.Scripts.Pool;
using Assets.Source.Scripts.Colorize;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Factory
{
    public class EntryColorListsFactory : MonoBehaviour
    {
        [SerializeField] private int _countOneColor;
        [SerializeField] private EntryListColorPool _pool;

        public void Initialize(
            IReadOnlyList<Color> beginColors,
            IReadOnlyList<Color> remainingColors)
        {
            _pool.Clear();

            foreach (Color color in beginColors)
            {
                ColorEntry entry = new ColorEntry(color, _countOneColor);

                _pool.AddBeginingColors(entry);
            }

            foreach (Color color in remainingColors)
            {
                ColorEntry entry = new ColorEntry(color, _countOneColor);

                _pool.AddRemainingColors(entry);
            }
        }

        public void Reset()
        {
            _pool.Clear();
        }
    }
}
