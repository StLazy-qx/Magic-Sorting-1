using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.UI.CanvasLight
{
    public class LightAnimation : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [Header("Alpha Range")]
        [SerializeField] private float _minAlpha;
        [SerializeField] private float _maxAlpha;
        [Header("Animation Settings")]
        [SerializeField] private float _duration;
        [SerializeField] private Ease _ease = Ease.InOutSine;

        private Tween _tween;

        private void Awake()
        {
            if (_image == null)
                _image = GetComponent<Image>();

            gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            StartAnimation();
        }

        private void OnDisable()
        {
            _tween?.Kill();
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        private void StartAnimation()
        {
            SetAlpha(_minAlpha);

            _tween = _image
                .DOFade(_maxAlpha, _duration)
                .SetEase(_ease)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void SetAlpha(float value)
        {
            //Color color = _image.color;
            //color.a = value;
            //_image.color = color;

            _image.color = new Color(
                _image.color.r, 
                _image.color.g, 
                _image.color.b, 
                value);
        }
    }
}
