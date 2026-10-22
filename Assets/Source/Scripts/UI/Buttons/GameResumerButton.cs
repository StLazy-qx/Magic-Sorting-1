using Assets.Source.Scripts.EntryPoint;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.UI.Buttons
{
    public class GameResumerButton : BaseButton
    {
        [SerializeField] private LoadingWindow _loadingWindow;

        protected override void OnButtonClick()
        {
            _loadingWindow.Show();
            GameHandler.ResumeGame();
            CurrentPanel?.Close();
        }

        public void SetInteractable(bool isAvailable)
        {
            if (Button == null)
                Button = GetComponent<Button>();

            Button.interactable = isAvailable;
        }
    }
}