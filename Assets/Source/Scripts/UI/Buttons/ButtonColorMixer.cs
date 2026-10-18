namespace Assets.Source.Scripts.UI.Buttons
{
    public class ButtonColorMixer : StatefulButton
    {
        public void Disable()
        {
            SetState(true);
        }

        public void Enable()
        {
            ResetState();
        }

        protected override void HandleClick() {}
    }
}