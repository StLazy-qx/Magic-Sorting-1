using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.GameDifficulty;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class RoundLauncher
    {
        private readonly ColumnsFactory _columnsFactory;
        private readonly VesselFactory _vesselFactory;
        private readonly EntryColorListsFactory _entryColorListsFactory;
        private readonly ColorColumnDistributor _columnDistributor;

        public RoundLauncher(
            ColumnsFactory columnsFactory,
            VesselFactory vesselFactory,
            EntryColorListsFactory entryColorListsFactory,
            ColorColumnDistributor columnDistributor)
        {
            _columnsFactory = columnsFactory ?? throw new ArgumentNullException(nameof(columnsFactory));
            _vesselFactory = vesselFactory ?? throw new ArgumentNullException(nameof(vesselFactory));
            _entryColorListsFactory = entryColorListsFactory ?? throw new ArgumentNullException(nameof(entryColorListsFactory));
            _columnDistributor = columnDistributor ?? throw new ArgumentNullException(nameof(columnDistributor));
        }

        public async UniTask LaunchAsync(
            ColorRandomizer colorRandomizer,
            DifficultySettings settings,
            DifficultyLevel difficulty,
            CancellationToken cancellationToken)
        {
            ResetFactories(difficulty);

            _vesselFactory.InitRandomizer(colorRandomizer);
            _entryColorListsFactory.Initialize(
                colorRandomizer.BeginColors,
                colorRandomizer.RemainingColors);
            _vesselFactory.Spawn();

            await UniTask.WaitUntil(() => _vesselFactory.IsReady, cancellationToken: cancellationToken);

            if (_vesselFactory.Objects != null && _vesselFactory.Objects.Count > 0)
            {
                _columnsFactory.Initialize(
                    _vesselFactory.Objects,
                    settings.ColumnsCount,
                    settings.MaxCellsPerColumn);
                _columnsFactory.Spawn();
            }

            _columnDistributor.Distribute();
        }

        private void ResetFactories(DifficultyLevel difficulty)
        {
            _entryColorListsFactory.Reset();
            _vesselFactory.ResetFactory(difficulty);
            _columnsFactory.ResetFactory(difficulty);
        }
    }
}
