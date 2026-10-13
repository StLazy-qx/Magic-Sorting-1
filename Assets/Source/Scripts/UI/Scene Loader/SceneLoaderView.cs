using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Source.Scripts.SceneManagement;
using Zenject;
using Assets.Source.Scripts.Extensions;

namespace Assets.Source.Scripts.UI.Scene_Loader
{
    public class SceneLoaderView : MonoBehaviour
    {
        private const float MinFillSpeed = 0.1f;
        private const float StartProgress = 0f;
        private const float PercentMultiplier = 100f;
        private const string PercentFormat = "{0} %";
        private const string InitialPercentText = "0 %";

        [SerializeField] private Image _fillImage;
        [SerializeField] private TMP_Text _percentText;
        [SerializeField, Min(0.1f)] private float _fillSpeed = 0.6f;

        private SceneLoader _sceneLoader;
        private float _displayedProgress;

        private void Awake()
        {
            Guard.NotNull(_fillImage, nameof(_fillImage));
            Guard.NotNull(_percentText, nameof(_percentText));
            Guard.InRange(_fillSpeed, MinFillSpeed, 
                float.MaxValue, nameof(_fillSpeed));
        }

        private void OnEnable()
        {
            _displayedProgress = StartProgress;

            _percentText.SetText(InitialPercentText);
            Render(StartProgress);
            _percentText.ForceMeshUpdate();
        }

        private void Start()
        {
            Render(_displayedProgress);
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
            _fillImage.fillAmount = progress;

            _percentText.SetText(PercentFormat, 
                Mathf.RoundToInt(progress * PercentMultiplier));
        }

        [Inject]
        private void Construct(SceneLoader sceneLoader)
        {
            Guard.NotNull(sceneLoader, nameof(sceneLoader));

            _sceneLoader = sceneLoader;
        }
    }
}