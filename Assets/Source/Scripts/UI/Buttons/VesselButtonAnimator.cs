using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace Assets.Source.Scripts.UI.Buttons
{
    public class VesselButtonAnimator
    {
        private Image _image;
        private Button _button;
        private Transform _transform;
        private Sequence _sequence;
        private Vector3 _initialScale;

        private readonly float _appearDuration = 0.3f;
        private readonly float _hideDuration = 0.2f;
        private readonly float _autoHideDelay = 3.0f;

        public void Initialize(Image image, Button button, Transform transform)
        {
            _image = image;
            _button = button;
            _transform = transform;
            _initialScale = _transform.localScale;

            HideImmediate();
        }

        public void Show(Action onAutoHide = null)
        {
            _sequence?.Kill();

            // Включаем интерактивность кнопки
            _button.interactable = true;

            // Сбрасываем в начальное состояние перед показом
            _transform.localScale = Vector3.zero;
            SetAlpha(0f);

            _sequence = DOTween.Sequence();

            // 1. Появление
            _sequence.Append(_transform.DOScale(_initialScale, _appearDuration).SetEase(Ease.OutBack));
            _sequence.Join(_image.DOFade(1f, _appearDuration));

            // 2. Ожидание 3 секунды
            _sequence.AppendInterval(_autoHideDelay);

            // 3. Исчезновение
            _sequence.Append(_transform.DOScale(Vector3.zero, _hideDuration).SetEase(Ease.InBack));
            _sequence.Join(_image.DOFade(0f, _hideDuration));

            // 4. Завершение
            _sequence.OnComplete(() =>
            {
                _button.interactable = false;
                onAutoHide?.Invoke();
            });
        }

        public void Hide()
        {
            _sequence?.Kill();

            _button.interactable = false;
            _sequence = DOTween.Sequence();

            _sequence.Append(_transform.DOScale(
                Vector3.zero, _hideDuration).SetEase(Ease.InBack));
            _sequence.Join(_image.DOFade(0f, _hideDuration));
        }

        public void HideImmediate()
        {
            _sequence?.Kill();
            _sequence = null;
            _button.interactable = false;

            SetAlpha(0f);

            _transform.localScale = _initialScale;
        }

        public void Dispose()
        {
            _sequence?.Kill();

            _sequence = null;
        }

        private void SetAlpha(float alpha)
        {
            Color color = _image.color;
            color.a = alpha;
            _image.color = color;
        }
    }
}