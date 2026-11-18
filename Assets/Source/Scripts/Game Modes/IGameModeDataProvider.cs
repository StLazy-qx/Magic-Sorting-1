using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.Player;

namespace Assets.Source.Scripts.Game_Modes
{
    public interface IGameModeDataProvider
    {
        public GameModeData Get(GameMode mode);
    }
}