namespace Assets.Source.Scripts.UI.Buttons
{
    public class ReverseButton : StatefulButton
    {
        public void Disable()
        {
            SetState(true);
        }

        public void Enable()
        {
            ResetState();
        }
    }
}
