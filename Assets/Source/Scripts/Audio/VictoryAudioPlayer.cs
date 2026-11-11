using UnityEngine;
using System;
using Assets.Source.Scripts.Vessels;
using System.Collections;

namespace Assets.Source.Scripts.Audio
{
    class VictoryAudioPlayer : MonoBehaviour
    {
        private const float ValueUntilAudioTurnsOn = 0.25f;

        [SerializeField] private VesselRoundController _vesselRoundController;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _interactClip;

        private void Awake()
        {
            if (_audioSource == null)
            {
                throw new ArgumentNullException
                    (nameof(_audioSource), "AudioSource cannot be null");
            }

            if (_vesselRoundController == null)
            {
                throw new ArgumentNullException
                    (nameof(_audioSource), "AudioSource cannot be null");
            }

            if (_interactClip == null)
                return;
        }

        private void OnEnable()
        {
            _vesselRoundController.VictoryAudioClipEnabled += OnPlayInteractSound;
        }

        private void OnDisable()
        {
            _vesselRoundController.VictoryAudioClipEnabled -= OnPlayInteractSound;
        }

        private void OnPlayInteractSound()
        {
            StartCoroutine(InvokeVictoryDuration());
        }

        private IEnumerator InvokeVictoryDuration()
        {
            yield return new WaitForSeconds(ValueUntilAudioTurnsOn);

            _audioSource.PlayOneShot(_interactClip);
        }
    }
}
