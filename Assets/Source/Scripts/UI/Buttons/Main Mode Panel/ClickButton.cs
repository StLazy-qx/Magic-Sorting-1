using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.UI.Buttons.Main_Mode_Panel
{
    [RequireComponent(typeof(Button))]

    public class ClickButton : MonoBehaviour
    {
        private Button _button;

        public event Action Clicked;

        private void Awake()
        {
            _button = GetComponent<Button>();

            if (_button == null)
                throw new MissingComponentException($"Button component is missing on {name}.");
        }

        private void OnEnable() 
            => _button.onClick.AddListener(HandleClick);

        private void OnDisable() 
            => _button.onClick.RemoveListener(HandleClick);

        private void HandleClick() 
            => Clicked?.Invoke();
    }
}
