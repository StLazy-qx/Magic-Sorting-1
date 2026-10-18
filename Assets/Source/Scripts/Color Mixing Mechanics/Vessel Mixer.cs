using Assets.Source.Scripts.Extensions;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.Color_Mixing_Mechanics
{
    public class VesselMixer : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _mixingEffect;

        public event Action Interacted;

        private void Awake()
        {
            Guard.NotNull(_mixingEffect, nameof(_mixingEffect));
            StopEffect();
        }

        private void OnMouseDown()
        {
            Interacted?.Invoke();
        }

        public void Activate()
        {
            if (_mixingEffect.isPlaying)
                return;

            _mixingEffect.Play();
        }

        public void Deactivate()
        {
            StopEffect();
        }

        private void StopEffect()
        {
            _mixingEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
