
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Vessels;
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

            Log("Constructor: все зависимости получены.");
        }

        private void Log(string message)
        {
            Debug.Log(
                $"[RoundLauncher][Frame:{Time.frameCount}] {message}");
        }

        private void Warn(string message)
        {
            Debug.LogWarning(
                $"[RoundLauncher][Frame:{Time.frameCount}] {message}");
        }

        private void Error(string message)
        {
            Debug.LogError(
                $"[RoundLauncher][Frame:{Time.frameCount}] {message}");
        }

        private string DescribeColors(IReadOnlyList<Color> colors)
        {
            if (colors == null)
                return "NULL";

            if (colors.Count == 0)
                return "EMPTY";

            string result = $"Count={colors.Count} [";

            for (int i = 0; i < colors.Count; i++)
            {
                if (i > 0)
                    result += ", ";

                result += $"#{i}={colors[i]}";
            }

            return result + "]";
        }

        public async UniTask LaunchAsync(
            ColorRandomizer colorRandomizer,
            DifficultySettings settings,
            DifficultyLevel difficulty,
            CancellationToken cancellationToken)
        {
            int id = ++_launchId;

            Log($"========== LAUNCH #{id} START ==========");
            Log($"Difficulty={difficulty}");

            try
            {
                // 1. Проверяем входные параметры
                Log("STEP 1: проверка входных параметров.");

                if (colorRandomizer == null)
                    throw new ArgumentNullException(nameof(colorRandomizer));

                if (settings == null)
                    throw new ArgumentNullException(nameof(settings));

                Log(
                    $"Settings: ColumnsCount={settings.ColumnsCount}, " +
                    $"MaxCellsPerColumn={settings.MaxCellsPerColumn}, " +
                    $"CountMixeColor={settings.CountMixeColor}");

                cancellationToken.ThrowIfCancellationRequested();

                // 2. Сброс фабрик
                Log("STEP 2: ResetFactories.");

                ResetFactories(difficulty);

                Log($"[Launch #{id}] ResetFactories завершён.");

                cancellationToken.ThrowIfCancellationRequested();

                // 3. Настройка рандомайзера сосудов
                Log("STEP 3: InitRandomizer.");

                Log(
                    $"BeginColors BEFORE: " +
                    $"{DescribeColors(colorRandomizer.BeginColors)}");

                Log(
                    $"RemainingColors BEFORE: " +
                    $"{DescribeColors(colorRandomizer.RemainingColors)}");

                _vesselFactory.InitRandomizer(colorRandomizer);

                Log("InitRandomizer завершён.");

                // 4. Инициализация списков цветов
                Log("STEP 4: EntryColorListsFactory.Initialize.");

                IReadOnlyList<Color> beginColors = colorRandomizer.BeginColors;
                IReadOnlyList<Color> remainingColors = colorRandomizer.RemainingColors;

                Log($"BeginColors: {DescribeColors(beginColors)}");
                Log($"RemainingColors: {DescribeColors(remainingColors)}");

                _entryColorListsFactory.Initialize(
                    beginColors,
                    remainingColors);

                Log("EntryColorListsFactory.Initialize завершён.");

                cancellationToken.ThrowIfCancellationRequested();

                // 5. Запуск спавна сосудов
                Log("STEP 5: VesselFactory.ForceSpawn.");

                Log(
                    $"Перед ForceSpawn: IsReady={_vesselFactory.IsReady}, " +
                    $"ObjectsCount={GetVesselCount()}");

                _vesselFactory.ForceSpawn();

                Log(
                    $"После ForceSpawn: IsReady={_vesselFactory.IsReady}, " +
                    $"ObjectsCount={GetVesselCount()}");

                // 6. Ожидание готовности
                Log($"STEP 6: ожидание IsReady. Launch #{id}");

                await UniTask.WaitUntil(
                    () => _vesselFactory.IsReady,
                    cancellationToken: cancellationToken);

                Log(
                    $"IsReady стал TRUE. ObjectsCount={GetVesselCount()}");

                cancellationToken.ThrowIfCancellationRequested();

                // 7. Создание колонок
                Log("STEP 7: ColumnsFactory.Initialize / ForceSpawn.");

                int vesselCount = GetVesselCount();

                if (vesselCount > 0)
                {
                    Log(
                        $"Перед Initialize колонок: сосудов={vesselCount}, " +
                        $"ColumnsCount={settings.ColumnsCount}, " +
                        $"MaxCellsPerColumn={settings.MaxCellsPerColumn}");

                    _columnsFactory.Initialize(
                        _vesselFactory.Objects,
                        settings.ColumnsCount,
                        settings.MaxCellsPerColumn);

                    Log("ColumnsFactory.Initialize завершён.");

                    cancellationToken.ThrowIfCancellationRequested();

                    _columnsFactory.ForceSpawn();

                    Log("ColumnsFactory.ForceSpawn завершён.");
                }
                else
                {
                    Error(
                        $"Launch #{id}: сосуды не созданы. " +
                        "ColumnsFactory пропущена.");
                    return;
                }

                cancellationToken.ThrowIfCancellationRequested();

                // 8. Распределение цветов
                Log("STEP 8: ColorColumnDistributor.Initialize.");

                Log($"CountMixeColor={settings.CountMixeColor}");

                _columnDistributor.Initialize(
                    _recipes,
                    _converter,
                    colorRandomizer.RoundColors,
                    settings.CountMixeColor);

                Log("ColorColumnDistributor.Initialize завершён.");

                cancellationToken.ThrowIfCancellationRequested();

                Log("STEP 9: ColorColumnDistributor.Distribute.");

                _columnDistributor.Distribute();

                Log("ColorColumnDistributor.Distribute завершён.");

                Log($"========== LAUNCH #{id} SUCCESS ==========");
            }
            catch (OperationCanceledException)
            {
                Warn($"Launch #{id}: операция отменена.");
                throw;
            }
            catch (Exception exception)
            {
                Error(
                    $"Launch #{id}: исключение на одном из этапов:\n{exception}");
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
            Log($"ResetFactories: начало. Difficulty={difficulty}");

            Log("Reset EntryColorListsFactory.");
            _entryColorListsFactory.Reset();

            Log("Reset VesselFactory.");
            _vesselFactory.ResetFactory(difficulty);

            Log("Reset ColumnsFactory.");
            _columnsFactory.ResetFactory(difficulty);

            Log(
                $"ResetFactories завершён. " +
                $"Vessel IsReady={_vesselFactory.IsReady}, " +
                $"ObjectsCount={GetVesselCount()}");
        }
    }
}
