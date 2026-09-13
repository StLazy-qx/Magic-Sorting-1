using Assets.Source.Scripts.EntryPoint;
using UnityEngine;

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
    }
}