using Assets.Source.Scripts.EntryPoint;
using UnityEngine;

namespace Assets.Source.Scripts.UI.Buttons
{
    public class MainMenuButton : BaseButton
    {
        [SerializeField] private LoadingWindow _loadingWindow;

        protected override void OnButtonClick()
        {
            CurrentPanel?.Close();
            _loadingWindow.Show();
            GameHandler.OpenMainMenu();
        }
    }
}