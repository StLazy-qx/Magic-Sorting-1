using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.Player;
using System;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Game_Modes
{
    public sealed class GameModeDataProvider : IGameModeDataProvider
    {
        private readonly Dictionary<GameMode, GameModeData> _byMode;
        private readonly IReadOnlyList<GameModeData> _all;

        public GameModeDataProvider()
            : this(GameModeDataCatalog.CreateAll()) { }

        public GameModeDataProvider(IReadOnlyList<GameModeData> all)
        {
            if (all == null)
                throw new ArgumentNullException(nameof(all));

            var byMode = new Dictionary<GameMode, GameModeData>(all.Count);

            for (int i = 0; i < all.Count; i++)
            {
                GameModeData data = all[i];

                if (data == null)
                    throw new ArgumentException($"GameModeData[{i}] == null.", nameof(all));

                if (Enum.IsDefined(typeof(GameMode), data.Mode) == false)
                    throw new ArgumentException(
                        $"GameModeData[{i}].Mode = {data.Mode} ({(int)data.Mode}) не является допустимым {nameof(GameMode)}.",
                        nameof(all));

                if (byMode.ContainsKey(data.Mode))
                    throw new ArgumentException(
                        $"Дубликат GameModeData для режима {data.Mode}.", nameof(all));

                byMode[data.Mode] = data;
            }

            _byMode = byMode;
            _all = all;
        }

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
