using UnityEngine;
using DG.Tweening;
using Assets.Source.Scripts.UI.GamePanel;

namespace Assets.Source.Scripts.UI.StoreView
{
    class ScoreWarningPanel : MonoBehaviour
    {
        [SerializeField] private Panel _panel;
        [SerializeField] private CanvasGroup _canvasGroup;

        private float _appearDuration = 0.4f;
        private float _showDelay = 1.4f;
        private float _hideDuration = 0.8f;
        private Sequence _sequence;
        private Vector3 _initialScale;

        private void Awake()
        {
            if (_panel == null)
                _panel = GetComponent<Panel>();

            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            _initialScale = transform.localScale;

            _panel.Close();
        }

        public void Show()
        {
            _sequence?.Kill();
            _panel.Open();

            transform.localScale = Vector3.zero;
            _canvasGroup.alpha = 1f;
            _sequence = DOTween.Sequence();

            _sequence.Append(transform.DOScale(_initialScale, _appearDuration)
                .SetEase(Ease.OutBack));
            _sequence.AppendInterval(_showDelay);
            _sequence.Append(transform.DOScale(Vector3.zero, _hideDuration)
                .SetEase(Ease.InBack));
            _sequence.Join(_canvasGroup.DOFade(0f, _hideDuration));

            _sequence.OnComplete(() =>
            {
                transform.localScale = _initialScale;
                _canvasGroup.alpha = 1f;

                _panel.Close();
            });
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }
    }
}
