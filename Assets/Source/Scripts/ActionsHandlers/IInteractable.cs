using System;

namespace Assets.Source.Scripts.ActionsHandlers
{
    public interface IInteractable
    {
        public event Action Interacted;

        public void OnClick();
    }
}