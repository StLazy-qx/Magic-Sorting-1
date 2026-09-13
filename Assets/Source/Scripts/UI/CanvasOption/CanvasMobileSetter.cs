using UnityEngine;

namespace Assets.Source.Scripts.UI.CanvasOption
{
    public class CanvasMobileSetter : MonoBehaviour
    {
        public bool IsActive => gameObject.activeSelf;

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}