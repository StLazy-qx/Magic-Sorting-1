using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.UI;
using Assets.Source.Scripts.Vessels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.ActionsHandlers
{
    public class VesselInteractionCoordinator : MonoBehaviour
    {
        [SerializeField] private VesselCloseButton _closeButton;

        private readonly Dictionary<Vessel, Action> _handlers = new();

        public void Subscribe(Vessel vessel)
        {
            Guard.NotNull(vessel, nameof(vessel));
            Guard.NotNull(_closeButton, nameof(_closeButton));

            if (_handlers.ContainsKey(vessel))
                return;

            Action handler = () => _closeButton.Show(vessel);

            _handlers.Add(vessel, handler);
            
            vessel.Interacted += handler;
        }

        public void Unsubscribe(Vessel vessel)
        {
            if (vessel == null)
                return;

            if (!_handlers.TryGetValue(vessel, out Action handler))
                return;

            vessel.Interacted -= handler;
            
            _handlers.Remove(vessel);
        }

        private void OnDestroy()
        {
            foreach (var pair in _handlers)
            {
                if (pair.Key != null)
                    pair.Key.Interacted -= pair.Value;
            }

            _handlers.Clear();
        }
    }
}