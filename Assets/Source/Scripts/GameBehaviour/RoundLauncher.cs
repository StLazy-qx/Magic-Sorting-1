using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.GameDifficulty;
using Cysharp.Threading.Tasks;
using System;
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
        private readonly ColorRecipes _recipes;
        private readonly ColorConverter _converter;

        private int _launchId;

        // >>> FIX 3: источник отмены для текущего запуска. Новый запуск отменяет предыдущий.
        private CancellationTokenSource _launchCts;

        public RoundLauncher(
            ColumnsFactory columnsFactory,
            VesselFactory vesselFactory,
            EntryColorListsFactory entryColorListsFactory,
            ColorColumnDistributor columnDistributor,
            ColorRecipes recipes,
            ColorConverter converter)
        {
            Guard.NotNull(columnsFactory, nameof(columnsFactory));
            Guard.NotNull(vesselFactory, nameof(vesselFactory));
            Guard.NotNull(entryColorListsFactory, nameof(entryColorListsFactory));
            Guard.NotNull(columnDistributor, nameof(columnDistributor));
            Guard.NotNull(recipes, nameof(recipes));
            Guard.NotNull(converter, nameof(converter));

            _columnsFactory = columnsFactory;
            _vesselFactory = vesselFactory;
            _entryColorListsFactory = entryColorListsFactory;
            _columnDistributor = columnDistributor;
            _recipes = recipes;
            _converter = converter;
        }

        // >>> FIX 3: НОВЫЙ метод, чтобы можно было отменить запуск снаружи
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
            // >>> FIX 3: отменяем предыдущий запуск ДО ResetFactories, иначе он
            // проснётся после WaitUntil и достроит колонки поверх нового раунда
            CancelCurrentLaunch();
            _launchCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            CancellationToken token = _launchCts.Token;

            int id = ++_launchId;

            try
            {
                if (colorRandomizer == null)
                    throw new ArgumentNullException(nameof(colorRandomizer));

                if (settings == null)
                    throw new ArgumentNullException(nameof(settings));

                token.ThrowIfCancellationRequested();
                ResetFactories(difficulty);
                token.ThrowIfCancellationRequested();
                _vesselFactory.InitRandomizer(colorRandomizer);

                IReadOnlyList<Color> beginColors = colorRandomizer.BeginColors;
                IReadOnlyList<Color> remainingColors = colorRandomizer.RemainingColors;

                _entryColorListsFactory.Initialize(
                    beginColors,
                    remainingColors);
                token.ThrowIfCancellationRequested();
                _vesselFactory.ForceSpawn();

                // ВАЖНО: если ResetFactory в VesselFactory синхронно не сбрасывает
                // IsReady и не очищает Objects, это условие выполнится сразу
                // и мы получим сосуды прошлого раунда. Проверить в VesselFactory.
                await UniTask.WaitUntil(
                    () => _vesselFactory.IsReady,
                    cancellationToken: token);

                token.ThrowIfCancellationRequested();

                // >>> FIX 3: теперь _launchId реально используется: запуск устарел
                if (id != _launchId)
                    return;

                int vesselCount = GetVesselCount();

                if (vesselCount > 0)
                {
                    _columnsFactory.Initialize(
                        _vesselFactory.Objects,
                        settings.ColumnsCount,
                        settings.MaxCellsPerColumn);
                    token.ThrowIfCancellationRequested();
                    _columnsFactory.ForceSpawn();
                }
                else
                {
                    return;
                }

                token.ThrowIfCancellationRequested();
                _columnDistributor.Initialize(
                    _recipes,
                    _converter,
                    colorRandomizer.RoundColors,
                    settings.CountMixeColor);
                token.ThrowIfCancellationRequested();
                _columnDistributor.Distribute();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);

                throw;
            }
        }

        private int GetVesselCount()
        {
            return _vesselFactory.Objects == null
                ? -1
                : _vesselFactory.Objects.Count;
        }

        private void ResetFactories(DifficultyLevel difficulty)
        {
            _entryColorListsFactory.Reset();
            _vesselFactory.ResetFactory(difficulty);
            _columnsFactory.ResetFactory(difficulty);
        }
    }
}

//namespace Assets.Source.Scripts.GameBehaviour
//{
//    public class RoundLauncher
//    {
//        private readonly ColumnsFactory _columnsFactory;
//        private readonly VesselFactory _vesselFactory;
//        private readonly EntryColorListsFactory _entryColorListsFactory;
//        private readonly ColorColumnDistributor _columnDistributor;
//        private readonly ColorRecipes _recipes;
//        private readonly ColorConverter _converter;

//        private int _launchId;

//        public RoundLauncher(
//            ColumnsFactory columnsFactory,
//            VesselFactory vesselFactory,
//            EntryColorListsFactory entryColorListsFactory,
//            ColorColumnDistributor columnDistributor,
//            ColorRecipes recipes,
//            ColorConverter converter)
//        {
//            Guard.NotNull(columnsFactory, nameof(columnsFactory));
//            Guard.NotNull(vesselFactory, nameof(vesselFactory));
//            Guard.NotNull(entryColorListsFactory, nameof(entryColorListsFactory));
//            Guard.NotNull(columnDistributor, nameof(columnDistributor));
//            Guard.NotNull(recipes, nameof(recipes));
//            Guard.NotNull(converter, nameof(converter));

//            _columnsFactory = columnsFactory;
//            _vesselFactory = vesselFactory;
//            _entryColorListsFactory = entryColorListsFactory;
//            _columnDistributor = columnDistributor;
//            _recipes = recipes;
//            _converter = converter;
//        }

//        public async UniTask LaunchAsync(
//            ColorRandomizer colorRandomizer,
//            DifficultySettings settings,
//            DifficultyLevel difficulty,
//            CancellationToken cancellationToken)
//        {
//            int id = ++_launchId;

//            try
//            {
//                if (colorRandomizer == null)
//                    throw new ArgumentNullException(nameof(colorRandomizer));

//                if (settings == null)
//                    throw new ArgumentNullException(nameof(settings));

//                cancellationToken.ThrowIfCancellationRequested();
//                ResetFactories(difficulty);
//                cancellationToken.ThrowIfCancellationRequested();
//                _vesselFactory.InitRandomizer(colorRandomizer);

//                IReadOnlyList<Color> beginColors = colorRandomizer.BeginColors;
//                IReadOnlyList<Color> remainingColors = colorRandomizer.RemainingColors;

//                _entryColorListsFactory.Initialize(
//                    beginColors,
//                    remainingColors);
//                cancellationToken.ThrowIfCancellationRequested();
//                _vesselFactory.ForceSpawn();

//                await UniTask.WaitUntil(
//                    () => _vesselFactory.IsReady,
//                    cancellationToken: cancellationToken);

//                cancellationToken.ThrowIfCancellationRequested();

//                int vesselCount = GetVesselCount();

//                if (vesselCount > 0)
//                {
//                    _columnsFactory.Initialize(
//                        _vesselFactory.Objects,
//                        settings.ColumnsCount,
//                        settings.MaxCellsPerColumn);
//                    cancellationToken.ThrowIfCancellationRequested();
//                    _columnsFactory.ForceSpawn();
//                }
//                else
//                {
//                    return;
//                }

//                cancellationToken.ThrowIfCancellationRequested();
//                _columnDistributor.Initialize(
//                    _recipes,
//                    _converter,
//                    colorRandomizer.RoundColors,
//                    settings.CountMixeColor);
//                cancellationToken.ThrowIfCancellationRequested();
//                _columnDistributor.Distribute();
//            }
//            catch (OperationCanceledException)
//            {
//                throw;
//            }
//            catch (Exception exception)
//            {
//                throw;
//            }
//        }

//        private int GetVesselCount()
//        {
//            return _vesselFactory.Objects == null
//                ? -1
//                : _vesselFactory.Objects.Count;
//        }

//        private void ResetFactories(DifficultyLevel difficulty)
//        {
//            _entryColorListsFactory.Reset();
//            _vesselFactory.ResetFactory(difficulty);
//            _columnsFactory.ResetFactory(difficulty);
//        }
//    }
//}
