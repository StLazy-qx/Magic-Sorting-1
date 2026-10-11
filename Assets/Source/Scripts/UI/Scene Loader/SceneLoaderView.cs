using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Source.Scripts.SceneManagement;

namespace Assets.Source.Scripts.UI.Scene_Loader
{
    public class SceneLoaderView : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TMP_Text _percentText;
        [SerializeField, Min(0.1f)] private float _fillSpeed = 1.5f;

        private SceneLoader _sceneLoader;
        private float _displayedProgress;

        public void Initialize(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
            _displayedProgress = 0f;

            Render(0f);
        }

        private void OnEnable()
        {
            _displayedProgress = 0f;

            Render(0f);
        }

        private void Update()
        {
            if (_sceneLoader == null || _sceneLoader.IsLoading == false)
                return;

            _displayedProgress = Mathf.MoveTowards(
                _displayedProgress,
                _sceneLoader.Progress,
                _fillSpeed * Time.unscaledDeltaTime);

            Render(_displayedProgress);
        }

        private void Render(float progress)
        {
            if (_fillImage != null)
                _fillImage.fillAmount = progress;

            if (_percentText != null)
                _percentText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
        }
    }
}