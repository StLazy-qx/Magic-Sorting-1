using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.Pool;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using Assets.Source.Scripts.Colorize;
using System;

namespace Assets.Source.Scripts.Factory
{
    public class ColorColumnDistributor : MonoBehaviour
    {
        [SerializeField] private MagicColumnPool _columnPool;
        [SerializeField] private EntryListColorPool _colorPool;

        public void Distribute()
        {
            Validate();

            while (TryGetRandomColumnWithSpace(out MagicColumn column))
            {
                ColorEntry entry = _colorPool.Get();

                if (entry == null)
                    break;

                Color color = entry.ConsumeColor();

                column.AddCell(color);
            }
        }

        private bool TryGetRandomColumnWithSpace(out MagicColumn column)
        {
            column = null;
            IReadOnlyList<MagicColumn> activeColumns = _columnPool.GetActiveObjects();

            if (activeColumns == null || activeColumns.Count == 0)
                return false;

            List<MagicColumn> available = activeColumns
                .Where(column => column.CanAddCell())
                .ToList();

            if (available.Count == 0)
                return false;

            column = available[Random.Range(0, available.Count)];

            return true;
        }

        private void Validate()
        {
            if (_columnPool == null)
                throw new ArgumentNullException(nameof(_columnPool));

            if (_colorPool == null)
                throw new ArgumentNullException(nameof(_colorPool));
        }
    }
}
