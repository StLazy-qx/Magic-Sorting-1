using Assets.Source.Scripts.Extensions;
using UnityEngine;
using System;

namespace Assets.Source.Scripts.Factory
{
    public class FactoryHolder : MonoBehaviour
    {
        [SerializeField] private ColumnsFactory _columns;
        [SerializeField] private VesselFactory _vessels;
        [SerializeField] private EntryColorListsFactory _entryColors;

        public ColumnsFactory Columns => _columns;
        public VesselFactory Vessels => _vessels;
        public EntryColorListsFactory EntryColors => _entryColors;

        private void Awake()
        {
            Guard.NotNull(_columns, nameof(_columns));
            Guard.NotNull(_vessels, nameof(_vessels));
            Guard.NotNull(_entryColors, nameof(_entryColors));
        }

        public void Initialize()
        {
            //_columns.Initialize();
            //_vessels.Initialize();
            //_entryColors.Initialize();
        }

        public void Reset()
        {
            //_columns.Reset();
            //_vessels.Reset();
            //_entryColors.Reset();
        }
    }
}