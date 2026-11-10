using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Save_Policies;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class RoundSection : BaseSection
    {
        private const int FirstRoundNumber = 1;

        public RoundSection(SavesYG savesContainer, ISavePolicy savePolicy)
            : base(savesContainer, savePolicy) { }

        public int ActualRoundNumber => _savesContainer.ActualRoundNumber;

        public void SaveNumber(int roundNumber)
        {
            Mutate(ref _savesContainer.ActualRoundNumber,
                roundNumber,
                validatedRoundNumber => 
                Guard.NotNegative(validatedRoundNumber, nameof(roundNumber)));
        }


        public int GetNumber()
        {
            return _savesContainer.ActualRoundNumber > 0
                ? _savesContainer.ActualRoundNumber
                : FirstRoundNumber;
        }

        // Удалить после отладки UI рейтингов
        public void DebugSeedRoundNumber(int roundNumber)
        {
            Guard.Positive(roundNumber, nameof(roundNumber));

            _savesContainer.ActualRoundNumber = roundNumber;

            _savePolicy.RequestSave();
        }
    }

    //public class RoundSection : BaseSection
    //{
    //    private const int FirstRoundNumber = 1;

    //    public RoundSection(SavesYG saves) : base(saves) { }

    //    public int ActualRoundNumber => _saves.ActualRoundNumber;

    //    public void SaveRoundNumber(int number)
    //    {
    //        Guard.NotNegative(number, nameof(number));

    //        if (_saves.ActualRoundNumber == number)
    //            return;

    //        _saves.ActualRoundNumber = number;

    //        SaveProgress();
    //    }

    //    public int GetRoundNumber()
    //    {
    //        return _saves.ActualRoundNumber > 0
    //            ? _saves.ActualRoundNumber
    //            : FirstRoundNumber;
    //    }

    //    // Удалить после отладки UI рейтингов
    //    public void DebugSeedRoundNumber(int number)
    //    {
    //        Guard.Positive(number, nameof(number));

    //        _saves.ActualRoundNumber = number;

    //        SaveProgress();
    //    }
    //}
}
