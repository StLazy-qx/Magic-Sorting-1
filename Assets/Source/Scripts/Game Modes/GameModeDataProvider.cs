using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.Player;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Game_Modes
{
    public sealed class GameModeDataProvider : IGameModeDataProvider
    {
        private readonly Dictionary<GameMode, GameModeData> _byMode;
        private readonly IReadOnlyList<GameModeData> _all;

        public IReadOnlyList<GameModeData> All => _all;

        public GameModeData Get(GameMode mode)
        {
            if (_byMode.TryGetValue(mode, out GameModeData data))
                return data;

            throw new KeyNotFoundException(
                $"GameModeData для режима {mode} ({(int)mode}) не зарегистрирован в {nameof(GameModeDataProvider)}.");
        }

        public bool TryGet(GameMode mode, out GameModeData data)
            => _byMode.TryGetValue(mode, out data);
    }
}
