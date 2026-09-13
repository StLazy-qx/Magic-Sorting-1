using UnityEngine;

namespace Assets.Source.Scripts.UI.GamePanel
{
    public class Panel : MonoBehaviour
    {
        public void Close()
            => gameObject.SetActive(false);

        public void Open()
            => gameObject.SetActive(true);
    }
}