using UnityEngine;
using UnityEngine.UI;
using Assets.Source.Scripts.Vessels;
using System;

namespace Assets.Source.Scripts.UI.Buttons
{
    public class VesselCloseButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;

        private VesselButtonAnimator _animator;
        private Vessel _currentVessel;

        public event Action Clicked;

        private void Awake()
        {
            if (_image == null)
                _image = GetComponent<Image>();

            _animator = new VesselButtonAnimator();

            _animator.Initialize(_image, _button, transform);
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        public void Show(Vessel vessel)
        {
            _currentVessel = vessel;
            _animator.Show();
        }

        public void Hide()
        {
            _currentVessel = null;
            _animator.Hide();
        }

        private void OnButtonClick()
        {
            Clicked?.Invoke();
            Hide();
        }

        //private void OnDestroy()
        //{
        //    _button.onClick.RemoveListener(OnButtonClick);
        //    _animator?.Dispose();
        //}
    }
}