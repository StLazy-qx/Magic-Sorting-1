using Assets.Source.Scripts.Player;
using UnityEngine;

namespace Assets.Source.Scripts.Game_Progress
{
    public class GameModePanel : MonoBehaviour
    {
        private const float LockedAlpha = 0.5f;
        private const float UnlockedAlpha = 1f;

        [SerializeField] private GameMode _mode;
        [SerializeField] private int _sceneIndex;
        [SerializeField] private bool _isOpen = true;

        [Header("Optional visuals")]
        [SerializeField] private CanvasGroup _canvasGroup;
        //[SerializeField] private GameObject _lockIcon; 

        public GameMode Mode => _mode;
        public int SceneIndex => _sceneIndex;
        public bool IsOpen => _isOpen;

        public void SetAvailable(bool isAvailable)
        {
            _isOpen = isAvailable;

            if (_canvasGroup != null)
                _canvasGroup.alpha = isAvailable ? UnlockedAlpha : LockedAlpha;

            //if (_lockIcon != null)
            //    _lockIcon.SetActive(isAvailable == false);
        }

        public void Open()
            => SetAvailable(true);

        public void Close()
            => SetAvailable(false);
    }
}