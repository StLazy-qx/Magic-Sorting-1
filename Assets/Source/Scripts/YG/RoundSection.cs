using Assets.Source.Scripts.Extensions;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class RoundSection
    {
        private const int FirstRoundNumber = 1;

        private readonly SavesYG _saves;

        public RoundSection(SavesYG saves)
        {
            _saves = saves;
        }

        public int ActualRoundNumber => _saves.ActualRoundNumber;

        public void SaveRoundNumber(int number)
        {
            Guard.NotNegative(number, nameof(number));

            if (_saves.ActualRoundNumber == number)
                return;

            _saves.ActualRoundNumber = number;

            SaveProgress();
        }

        public int GetRoundNumber()
        {
            return _saves.ActualRoundNumber > 0
                ? _saves.ActualRoundNumber
                : FirstRoundNumber;
        }

        public void SaveProgress()
        {
            YG2.SaveProgress();
        }
    }
}
