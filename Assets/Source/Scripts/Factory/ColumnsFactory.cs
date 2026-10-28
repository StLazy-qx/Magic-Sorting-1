using System.Collections.Generic;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.Vessels;
using Assets.Source.Scripts.Pool;
using Assets.Source.Scripts.Extensions;
using UnityEngine;
using System;

namespace Assets.Source.Scripts.Factory
{
    public class ColumnsFactory : Factory<MagicColumn>
    {
        [SerializeField] private MagicColumnPool _columnPool;
        [SerializeField] private MagicCellRouter _distributerMagicCell;
        [SerializeField] private ShuffledColorDistributor _colorDistributor;
        [SerializeField] private EntryListColorPool _entryListColorPool;

        private int _spawnPointCount;
        private int _maxCellsPerColumn;

        public void Initialize(
            IReadOnlyList<Vessel> vessels,
            int spawnPointCount,
            int maxCellsPerColumn)
        {
            if (spawnPointCount <= 0)
                throw new ArgumentNullException(nameof(spawnPointCount));
            if (maxCellsPerColumn <= 0)
                throw new ArgumentNullException(nameof(maxCellsPerColumn));
            if (_distributerMagicCell == null)
                throw new ArgumentNullException(nameof(_distributerMagicCell));
            if (_colorDistributor == null)
                throw new ArgumentNullException(nameof(_colorDistributor));
            if (_columnPool == null)
                throw new ArgumentNullException(nameof(_columnPool));

            _spawnPointCount = spawnPointCount;
            _maxCellsPerColumn = maxCellsPerColumn;

            _colorDistributor.Initialize(vessels);
            _distributerMagicCell.Initialize(vessels);
        }

        public override void ResetFactory(DifficultyLevel difficulty)
        {
            base.ResetFactory(difficulty);
        }

        public void ForceSpawn()
        {
            BuildObjects();
        }

        protected override void BuildObjects()
        {
            if (_columnPool == null)
                throw new ArgumentNullException(nameof(_columnPool));

            _columnPool.DeactivateAll();

            int instantiated = 0;
            int reactivated = 0;

            for (int i = 0; i < _spawnPointCount; i++)
            {
                if (i >= SpawnPoints.Length)
                    break;

                Transform point = SpawnPoints[i];
                MagicColumn column = _columnPool.Activate();

                if (column == null)
                {
                    column = Instantiate(Prefab, point.position,
                        point.rotation, _columnPool.Container);

                    _columnPool.Add(column);
                    column.gameObject.SetActive(true);
                    instantiated++;
                }
                else
                {
                    column.transform
                        .SetPositionAndRotation(point.position, point.rotation);

                    reactivated++;
                }

                column.Initialize(
                    _distributerMagicCell,
                    _colorDistributor,
                    _entryListColorPool,
                    _maxCellsPerColumn);
            }
        }
    }
}