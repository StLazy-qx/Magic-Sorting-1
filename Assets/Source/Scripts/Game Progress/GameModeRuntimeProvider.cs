using Assets.Source.Scripts.Player;
using System;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Game_Progress
{
    public sealed class GameModeRuntimeProvider
    {
        private readonly Dictionary<GameMode, GameModeRuntime> _modes = new();

        public GameModeRuntimeProvider(IEnumerable<GameModeData> modeDatas)
        {
            if (modeDatas == null)
                throw new ArgumentNullException(nameof(modeDatas));

            foreach (GameModeData data in modeDatas)
            {
                if (data == null) continue;
                _modes[data.Mode] = new GameModeRuntime(data);
            }
        }

        public GameModeRuntime Get(GameMode mode)
        {
            if (_modes.TryGetValue(mode, out GameModeRuntime runtime) == false)
                throw new KeyNotFoundException($"GameModeData для {mode} не зарегистрирован.");

            return runtime;
        }

        public bool TryGet(GameMode mode, out GameModeRuntime runtime)
            => _modes.TryGetValue(mode, out runtime);

        public void ResetMode(GameMode mode)
        {
            if (_modes.TryGetValue(mode, out GameModeRuntime runtime))
                runtime.ResetAll();
        }

        public void ResetAll()
        {
            foreach (var pair in _modes)
                pair.Value.ResetAll();
        }
    }
}