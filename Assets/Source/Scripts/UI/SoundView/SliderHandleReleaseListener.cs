using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Source.Scripts.UI.SoundView
{
    class SliderHandleReleaseListener : MonoBehaviour, IPointerUpHandler
    {
        public event Action Released;

        public void OnPointerUp(PointerEventData eventData)
        {
            Released?.Invoke();
        }
    }
}
