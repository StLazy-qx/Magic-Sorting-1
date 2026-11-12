using Assets.Source.Scripts.Game_Progress;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Game_Modes
{
    public static class GameModeDataCatalog
    {
        public static IReadOnlyList<GameModeData> CreateAll()
        {
            return new GameModeData[]
            {
                new TutorialGameModeData(),
                new EasyGameModeData(),
                new MediumGameModeData(),
                new HardGameModeData(),
                new FreeGameModeData(),
            };
        }
    }
}
