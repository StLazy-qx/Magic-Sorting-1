using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.Player;
using System;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Game_Modes
{
    public sealed class GameModeDataProvider : IGameModeDataProvider
    {
        private readonly Dictionary<GameMode, GameModeData> _byMode = new();

        public GameModeDataProvider()
        {
            GameMode[] none = Array.Empty<GameMode>();

            Register(GameModeData.Fixed(GameMode.Tutorial, isRated: false, none,
                DifficultyLevel.Easy,
                DifficultyLevel.MediumEasy));
            Register(GameModeData.Fixed(GameMode.Easy, isRated: true, none,
                DifficultyLevel.Easy, DifficultyLevel.Easy, DifficultyLevel.Easy,
                DifficultyLevel.MediumEasy, DifficultyLevel.MediumEasy, DifficultyLevel.MediumEasy));
            Register(GameModeData.Fixed(GameMode.Medium, isRated: true, new[] { GameMode.Easy },
                DifficultyLevel.Medium, DifficultyLevel.Medium, DifficultyLevel.Medium,
                DifficultyLevel.Medium, DifficultyLevel.Medium, DifficultyLevel.Medium));
            Register(GameModeData.Fixed(GameMode.Hard, isRated: true, new[] { GameMode.Medium },
                DifficultyLevel.MediumHard, DifficultyLevel.MediumHard, DifficultyLevel.MediumHard,
                DifficultyLevel.Hard, DifficultyLevel.Hard, DifficultyLevel.Hard));
            Register(GameModeData.Endless(GameMode.FreeMode,
                new[] { GameMode.Easy, GameMode.Medium, GameMode.Hard }));
        }

        public GameModeData Get(GameMode mode)
        {
            if (_byMode.TryGetValue(mode, out GameModeData data))
                return data;

            throw new KeyNotFoundException(
                $"GameModeData для режима {mode} ({(int)mode}) не зарегистрирован в {nameof(GameModeDataProvider)}.");
        }

        private void Register(GameModeData data)
            => _byMode.Add(data.Mode, data);
    }
}