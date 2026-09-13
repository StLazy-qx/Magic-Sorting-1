using YG;

namespace Assets.Source.Scripts.YG
{
    public abstract class BaseSection
    {
        protected readonly SavesYG _saves;

        protected BaseSection(SavesYG saves)
        {
            _saves = saves;
        }

        protected void SaveProgress()
        {
            YG2.SaveProgress();
        }
    }
}
