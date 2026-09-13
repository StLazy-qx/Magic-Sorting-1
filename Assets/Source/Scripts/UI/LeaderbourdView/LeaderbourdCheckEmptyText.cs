using Assets.Source.Scripts.Extensions;
using UnityEngine;
using TMPro;


namespace Assets.Source.Scripts.UI.LeaderbourdView
{
    class LeaderbourdCheckEmptyText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private void Awake()
        {
            Guard.NotNull(_text, nameof(_text));
        }

        public void ShowText()
            => _text.gameObject.SetActive(true);

        public void HideText()
            => _text.gameObject.SetActive(false);
    }
}
