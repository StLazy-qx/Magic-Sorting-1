using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Save_Policies;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class ScoreSection : BaseSection
    {
        public ScoreSection(SavesYG savesContainer, ISavePolicy savePolicy)
            : base(savesContainer, savePolicy) { }

        public int Points => _savesContainer.MainPoints;

        public void Save(int points)
        {
            Guard.NotNegative(points, nameof(points));

            _savesContainer.MainPoints = points;

            if (YG2.isSDKEnabled)
                YG2.SetLeaderboard("GameLeaderboard", _savesContainer.MainPoints);

            _savePolicy.RequestSave();
        }

        public void Decrease(int points)
            => Mutate(
                ref _savesContainer.MainPoints,
                points,
                validatedPoints => Guard.NotNegative(validatedPoints, nameof(points)));
    }

    //public class ScoreSection : BaseSection
    //{
    //    public ScoreSection(SavesYG saves) : base(saves) { }

    //    public int Points => _saves.MainPoints;

    //    public void SaveScore(int points)
    //    {
    //        Guard.NotNegative(points, nameof(points));

    //        _saves.MainPoints = points;

    //        if (YG2.isSDKEnabled)
    //            YG2.SetLeaderboard("GameLeaderboard", _saves.MainPoints);

    //        SaveProgress();
    //    }

    //    public void DecreaseScore(int points)
    //    {
    //        Guard.NotNegative(points, nameof(points));

    //        _saves.MainPoints = points;

    //        SaveProgress();
    //    }
    //}
}
