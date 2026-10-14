using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Tutorial;
using Assets.Source.Scripts.Vessels;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.UI
{
    public class VesselCloseButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private float _autoHideDelay = 4f;

        private readonly AsyncTimer _autoHideTimer = new();

        private Vessel _vessel;

        private void Awake()
        {
            Guard.NotNull(_button, nameof(_button));
            Guard.Positive(_autoHideDelay, nameof(_autoHideDelay));
            Hide();
        }

        public void Show(Vessel vessel)
        {
            Guard.NotNull(vessel, nameof(vessel));
            Hide();

            _vessel = vessel;
            _vessel.Filled += OnVesselFilled;

            _button.onClick.AddListener(OnButtonClick);
            gameObject.SetActive(true);
            _autoHideTimer.StartTimer(_autoHideDelay, Hide);
        }

        public void Hide()
        {
            _autoHideTimer.StopTimer();

            if (_vessel != null)
                _vessel.Filled -= OnVesselFilled;

            if (_button != null)
                _button.onClick.RemoveListener(OnButtonClick);

            _vessel = null;
            
            gameObject.SetActive(false);
        }

        private void OnDestroy() 
			=> _autoHideTimer.StopTimer();

        private void OnVesselFilled(Vector3 _) 
			=> Hide();

        private void OnButtonClick()
        {
            Vessel vessel = _vessel;

            Hide();

            vessel?.CompleteVessel();
        }
    }
}