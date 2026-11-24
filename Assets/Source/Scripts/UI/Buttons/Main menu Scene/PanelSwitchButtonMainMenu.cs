using System;
using Assets.Source.Scripts.UI.GamePanel;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.UI.Buttons.Main_menu_Scene
{
    [RequireComponent(typeof(Button))]

    public class PanelSwitchButtonMainMenu : MonoBehaviour
    {
        [SerializeField] private Panel _targetPanel;

        private Panel _currentPanel;
        private Button _button;

        public event Action Clicked;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _currentPanel = GetComponentInParent<Panel>();

            if (_currentPanel == null)
            {
                throw new MissingComponentException(
                    $"No Panel component found in parent objects of {name}.");
            }

            if (_targetPanel == null)
            {
                throw new NullReferenceException(
                    $"TargetPanel reference is missing in {nameof(PanelSwitchButton)} on {name}.");
            }
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            _currentPanel.Close();
            _targetPanel.Open();

            Clicked?.Invoke();
        }
    }
}
