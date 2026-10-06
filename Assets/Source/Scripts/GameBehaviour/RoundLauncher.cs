using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.GameDifficulty;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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
            Guard.NotNull(columnsFactory, nameof(columnsFactory));
            Guard.NotNull(vesselFactory, nameof(vesselFactory));
            Guard.NotNull(entryColorListsFactory, 
                nameof(entryColorListsFactory));
            Guard.NotNull(columnDistributor, nameof(columnDistributor));

            _columnsFactory = columnsFactory;
            _vesselFactory = vesselFactory;
            _entryColorListsFactory = entryColorListsFactory;
            _columnDistributor = columnDistributor;
        }

        public async UniTask LaunchAsync(
            ColorRandomizer colorRandomizer,
            DifficultySettings settings,
            TutorialLevelCounter tutorialLevelCounter,
            DifficultyLevel difficulty,
            CancellationToken cancellationToken)
        {
            ResetFactories(difficulty);

            _vesselFactory.InitRandomizer(colorRandomizer);

            IReadOnlyList<Color> beginColors = colorRandomizer.BeginColors;
            IReadOnlyList<Color> remainingColors = colorRandomizer.RemainingColors;

            if (tutorialLevelCounter.IsLastRound)
            {
                (beginColors, remainingColors) =(remainingColors, beginColors);
            }

            _entryColorListsFactory.Initialize(beginColors, remainingColors);
            _vesselFactory.Spawn();

            await UniTask.WaitUntil(() => _vesselFactory.IsReady,
                cancellationToken: cancellationToken);

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
