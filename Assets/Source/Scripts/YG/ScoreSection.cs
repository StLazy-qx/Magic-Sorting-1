using Assets.Source.Scripts.Extensions;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class ScoreSection
    {
        private readonly SavesYG _saves;

        public ScoreSection(SavesYG saves)
        {
            _saves = saves;
        }

        public int Points => _saves.MainPoints;

        public void SaveScore(int points)
        {
            Guard.NotNegative(points, nameof(points));

            _saves.MainPoints = points;

            if (YG2.isSDKEnabled)
                YG2.SetLeaderboard("GameLeaderboard", _saves.MainPoints);

            SaveProgress();
        }

        public void DecreaseScore(int points)
        {
            Guard.NotNegative(points, nameof(points));

            _saves.MainPoints = points;

            SaveProgress();
        }

        public void SaveProgress()
        {
            YG2.SaveProgress();
        }
    }
}
