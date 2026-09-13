namespace Assets.Source.Scripts.EntryPoint
{
    public interface IObjectInitilizable
    {
        public bool IsInitialized { get; }

        public void Initialize();
    }
}