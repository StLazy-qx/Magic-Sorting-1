using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.Player;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Game_Modes
{
    public interface IGameModeDataProvider
    {
        public IReadOnlyList<GameModeData> All { get; }
        public GameModeData Get(GameMode mode);
        public bool TryGet(GameMode mode, out GameModeData data);
    }
}
