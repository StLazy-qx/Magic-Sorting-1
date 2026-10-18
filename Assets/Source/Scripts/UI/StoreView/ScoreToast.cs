using UnityEngine;
using DG.Tweening;

namespace Assets.Source.Scripts.UI.StoreView
{
    public class ScoreToast : MonoBehaviour
    {
        [SerializeField] private UITemporaryView _UIObject;
        [SerializeField] private CanvasGroup _canvasGroup;

        private float _appearDuration = 0.4f;
        private float _showDelay = 1.4f;
        private float _hideDuration = 0.8f;
        private Sequence _sequence;
        private Vector3 _initialScale;

        private void Awake()
        {
            if (_UIObject == null)
                _UIObject = GetComponentInChildren<UITemporaryView>(true);

            if (_canvasGroup == null)
                _canvasGroup = _UIObject.GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            _initialScale = _UIObject.transform.localScale;

            _UIObject.gameObject.SetActive(false);
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

            _sequence.OnComplete(() =>
            {
                _UIObject.transform.localScale = _initialScale;
                _canvasGroup.alpha = 1f;

                _UIObject.gameObject.SetActive(false);
            });
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }
    }
}