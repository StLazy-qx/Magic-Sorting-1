using UnityEngine;
using DG.Tweening;

namespace Assets.Source.Scripts.UI.StoreView
{
    public class ScoreToast : MonoBehaviour
    {
        [SerializeField] private UITemporaryView _UIObject;

        private CanvasGroup _canvasGroup;
        private float _appearDuration = 0.4f;
        private float _showDelay = 1.4f;
        private float _hideDuration = 0.8f;
        private Sequence _sequence;
        private Vector3 _initialScale;

        private void Awake()
        {
            if (_UIObject == null)
                _UIObject = GetComponentInChildren<UITemporaryView>(true);

            if (!_UIObject.TryGetComponent(out _canvasGroup))
                _canvasGroup = _UIObject.gameObject.AddComponent<CanvasGroup>();

            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            _initialScale = _UIObject.transform.localScale;

            Hide();
        }

        public void Show()
        {
            _sequence?.Kill();
            _UIObject.gameObject.SetActive(true);

            _UIObject.transform.localScale = Vector3.zero;
            _canvasGroup.alpha = 1f;
            _sequence = DOTween.Sequence();

            _sequence.Append(_UIObject.transform.DOScale(_initialScale, _appearDuration)
                .SetEase(Ease.OutBack));
            _sequence.AppendInterval(_showDelay);
            _sequence.Append(_UIObject.transform.DOScale(Vector3.zero, _hideDuration)
                .SetEase(Ease.InBack));
            _sequence.Join(_canvasGroup.DOFade(0f, _hideDuration));
            _sequence.OnComplete(Hide);
        }

        public void Hide()
        {
            _sequence?.Kill();

            _sequence = null;

            if (_UIObject == null)
                return;

            _UIObject.transform.localScale = _initialScale;
            _canvasGroup.alpha = 1f;

            _UIObject.gameObject.SetActive(false);
        }

        private void OnDisable()
            => Hide();

        private void OnDestroy()
            => _sequence?.Kill();
    }
}