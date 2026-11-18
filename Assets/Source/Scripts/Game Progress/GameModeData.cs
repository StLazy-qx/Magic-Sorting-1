using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Player;
using System;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Game_Progress
{
    public sealed class GameModeData
    {
        private const int DefaultEndlessStep = 6;

        private readonly DifficultyLevel[] _fixedPlan;
        private readonly DifficultyLevel[] _endlessLevels;
        private readonly int _endlessStep;

        private GameModeData(
            GameMode mode,
            bool isRated,
            GameMode[] requiredModes,
            DifficultyLevel[] fixedPlan,
            int endlessStep)
        {
            Guard.NotNull(requiredModes, nameof(requiredModes));

            Mode = mode;
            IsRated = isRated;
            RequiredModes = requiredModes;
            _fixedPlan = fixedPlan;
            _endlessStep = endlessStep;

            if (fixedPlan == null)
            {
                _endlessLevels = (DifficultyLevel[])Enum.
                    GetValues(typeof(DifficultyLevel));

                Array.Sort(_endlessLevels);
            }
        }

        public GameMode Mode { get; }
        public bool IsRated { get; }
        public IReadOnlyList<GameMode> RequiredModes { get; }
        public bool IsEndless => _fixedPlan == null;
        public int RoundsCount => IsEndless ? int.MaxValue : _fixedPlan.Length;

        public static GameModeData Fixed(
            GameMode mode,
            bool isRated,
            GameMode[] requiredModes,
            params DifficultyLevel[] plan)
        {
            Guard.NotNullOrEmpty(plan, nameof(plan));

            return new GameModeData(
                mode, 
                isRated, 
                requiredModes, 
                (DifficultyLevel[])plan.Clone(), 
                0);
        }

        public static GameModeData Endless(
            GameMode mode,
            GameMode[] requiredModes,
            int difficultyStep = DefaultEndlessStep)
        {
            Guard.Positive(difficultyStep, nameof(difficultyStep));

            return new GameModeData(
                mode, 
                false, 
                requiredModes, 
                null, 
                difficultyStep);
        }

        public DifficultyLevel GetDifficulty(int roundIndex)
        {
            if (IsEndless)
            {
                Guard.NotNegative(roundIndex, nameof(roundIndex));

                return _endlessLevels[(roundIndex / _endlessStep) % _endlessLevels.Length];
            }

            Guard.InRange(roundIndex, 0, _fixedPlan.Length - 1, nameof(roundIndex));

            return _fixedPlan[roundIndex];
        }
    }
}