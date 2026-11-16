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
        [SerializeField] private CanvasGroup _canvasGroup;

        public GameMode Mode => _mode;
        public int SceneIndex => _sceneIndex;

        public void SetAvailable(bool isAvailable)
        {
            if (_canvasGroup != null)
                _canvasGroup.alpha = isAvailable ? UnlockedAlpha : LockedAlpha;
        }
    }
}