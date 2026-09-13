using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Assets.Source.Scripts.UI.Buttons
{
    public class ButtonPulseAnimation : MonoBehaviour
    {
        [SerializeField] protected Button Button;

        private float _animationDuration = 3f;
        private int _pulsesCount = 6;
        private float _scaleMultiplier = 1.05f;
        private Tween _tween;
        private Vector3 _defaultScale;

        private void Awake()
        {
            if (Button == null)
                return;

            _defaultScale = Button.transform.localScale;
        }

        public void Play()
        {
            if (Button == null)
                return;

            Stop();

            float onePulseDuration = _animationDuration / _pulsesCount;

            _tween = Button.transform
                .DOScale(_defaultScale * _scaleMultiplier, onePulseDuration)
                .SetEase(Ease.OutQuad)
                .SetLoops(_pulsesCount, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    Button.transform.localScale = _defaultScale;
                });
        }

        public void Stop()
        {
            Button.transform.DOKill();

            Button.transform.localScale = _defaultScale;
        }
    }
}
