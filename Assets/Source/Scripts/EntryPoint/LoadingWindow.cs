using UnityEngine;

namespace Assets.Source.Scripts.EntryPoint
{
    public class LoadingWindow : MonoBehaviour
    {
        private void Awake()
        {
            Hide();
        }

        public void Show()
        {
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}