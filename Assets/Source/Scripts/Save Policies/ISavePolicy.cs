namespace Assets.Source.Scripts.Save_Policies
{
    public interface ISavePolicy
    {
        public void RequestSave();
        public void Flush();
    }
}

