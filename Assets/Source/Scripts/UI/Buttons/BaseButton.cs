using Assets.Source.Scripts.UI.GamePanel;
using Assets.Source.Scripts.GameBehaviour;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Assets.Source.Scripts.UI.Buttons
{
    [RequireComponent(typeof(Button))]

    public abstract class BaseButton : MonoBehaviour
    {
        [SerializeField] protected BaseGameHandler GameHandler;
        [SerializeField] protected Panel TargetPanel;

        protected Panel CurrentPanel;
        protected Button Button;

        public event Action Clicked;

        private void Awake()
        {
            if (GameHandler == null)
            {
                throw new NullReferenceException(
                    "GameHandler reference is missing in BaseMenuButton.");
            }

            CurrentPanel = GetComponentInParent<Panel>();

            if (CurrentPanel == null)
            {
                throw new MissingComponentException(
                    "No Panel component found in parent objects.");
            }

            Button = GetComponent<Button>();

            if (Button == null)
            {
                throw new MissingComponentException(
                    "Button component is missing on this GameObject.");
            }
        }

        private void OnEnable()
        {
            Button.onClick.AddListener(HandleClick);
        }

        private void OnDisable()
        {
            Button.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            OnButtonClick();
            Clicked?.Invoke();
        }

        protected abstract void OnButtonClick();
    }
}
