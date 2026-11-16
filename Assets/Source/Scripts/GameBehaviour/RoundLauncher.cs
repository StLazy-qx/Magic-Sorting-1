using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Vessels;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class RoundLauncher
    {
        private readonly ColumnsFactory _columnsFactory;
        private readonly VesselFactory _vesselFactory;
        private readonly EntryColorListsFactory _entryColorListsFactory;
        private readonly ColorColumnDistributor _columnDistributor;
        private readonly ColorRecipes _recipes;
        private readonly ColorConverter _converter;
        private readonly Action<IReadOnlyList<Vessel>> _onVesselsReady;

        private CancellationTokenSource _launchCts;

        public RoundLauncher(
            ColumnsFactory columnsFactory,
            VesselFactory vesselFactory,
            EntryColorListsFactory entryColorListsFactory,
            ColorColumnDistributor columnDistributor,
            ColorRecipes recipes,
            ColorConverter converter,
            Action<IReadOnlyList<Vessel>> onVesselsReady = null)
        {
            _columnsFactory = columnsFactory;
            _vesselFactory = vesselFactory;
            _entryColorListsFactory = entryColorListsFactory;
            _columnDistributor = columnDistributor;
            _recipes = recipes;
            _converter = converter;
            _onVesselsReady = onVesselsReady;
        }

        public void CancelCurrentLaunch()
        {
            _launchCts?.Cancel();
            _launchCts?.Dispose();
            _launchCts = null;
        }

        public async UniTask LaunchAsync(
            ColorRandomizer colorRandomizer,
            DifficultySettings settings,
            DifficultyLevel difficulty,
            CancellationToken cancellationToken)
        {
            Guard.NotNull(colorRandomizer, nameof(colorRandomizer));
            Guard.NotNull(settings, nameof(settings));
            CancelCurrentLaunch();

            _launchCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            CancellationToken token = _launchCts.Token;

            _entryColorListsFactory.Reset();
            _vesselFactory.ResetFactory(difficulty);
            _columnsFactory.ResetFactory(difficulty);
            _vesselFactory.InitRandomizer(colorRandomizer);
            _entryColorListsFactory.Initialize(
                colorRandomizer.BeginColors,
                colorRandomizer.RemainingColors);
            _vesselFactory.ForceSpawn();

            await UniTask.WaitUntil(() => 
            _vesselFactory.IsReady, cancellationToken: token);

            if (_vesselFactory.Objects == null || _vesselFactory.Objects.Count == 0)
                return;

            _onVesselsReady?.Invoke(_vesselFactory.Objects);
            _columnsFactory.Initialize(
                _vesselFactory.Objects,
                settings.ColumnsCount,
                settings.MaxCellsPerColumn);
            _columnsFactory.ForceSpawn();
            _columnDistributor.Initialize(
                _recipes,
                _converter,
                colorRandomizer.RoundColors,
                settings.CountMixeColor);
            _columnDistributor.Distribute();
        }
    }
}