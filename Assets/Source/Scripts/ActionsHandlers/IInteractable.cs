using System;

namespace Assets.Source.Scripts.ActionsHandlers
{
    public interface IInteractable
    {
        event Action Interacted;

        void OnClick();
    }
}