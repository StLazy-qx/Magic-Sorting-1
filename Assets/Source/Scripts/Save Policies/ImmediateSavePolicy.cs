using YG;

namespace Assets.Source.Scripts.Save_Policies
{
    public sealed class ImmediateSavePolicy : ISavePolicy
    {
        public void RequestSave()
            => YG2.SaveProgress();

        public void Flush() { }
    }
}