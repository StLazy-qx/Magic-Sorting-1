using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.SceneManagement
{
    public class LoadingSceneLoader : MonoBehaviour
    {
        //отделить логику от UI в сцене загрузки
        [SerializeField] private Slider _progressBar;
        [SerializeField] private GameObject _loadingVisuals;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration = 0.3f;

        private static LoadingSceneLoader _instance;
        private bool _isLoading;
        private Coroutine _fadeCoroutine;

        public static LoadingSceneLoader Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Initialize()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            HideImmediate();
        }

        public void ShowLoadingScreen()
        {
            if (_isLoading) return;

            _isLoading = true;
            _loadingVisuals.SetActive(true);

            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeLoadingScreen(0f, 1f, _fadeDuration));
        }

        public void HideLoadingScreen()
        {
            if (!_isLoading) return;

            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeLoadingScreen(1f, 0f, _fadeDuration, () =>
            {
                _loadingVisuals.SetActive(false);
                _isLoading = false;
            }));
        }

        public void HideImmediate()
        {
            if (_canvasGroup != null)
                _canvasGroup.alpha = 0f;

            _loadingVisuals.SetActive(false);
            _isLoading = false;
        }

        public void UpdateProgress(float progress)
        {
            _progressBar.value = progress;
        }

        private IEnumerator FadeLoadingScreen(float startAlpha, float targetAlpha, float duration, Action onComplete = null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);

                if (_canvasGroup != null)
                    _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

                yield return null;
            }

            if (_canvasGroup != null)
                _canvasGroup.alpha = targetAlpha;

            onComplete?.Invoke();
            _fadeCoroutine = null;
        }
    }
}