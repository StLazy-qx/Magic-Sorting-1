using UnityEngine;
using System;
using Assets.Source.Scripts.Vessels;
using System.Collections;

namespace Assets.Source.Scripts.Audio
{
    class VictoryAudioPlayer : MonoBehaviour
    {
        private const float ValueUntilAudioTurnsOn = 0.25f;

        [SerializeField] private VesselStateTracker _vesselStateTracker;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _interactClip;

        private void Awake()
        {
            if (_audioSource == null)
            {
                throw new ArgumentNullException
                    (nameof(_audioSource), "AudioSource cannot be null");
            }

            if (_vesselStateTracker == null)
            {
                throw new ArgumentNullException
                    (nameof(_audioSource), "AudioSource cannot be null");
            }

            if (_interactClip == null)
                return;
        }

        private void OnEnable()
        {
            _vesselStateTracker.VictoryAudioClipEnabled += OnPlayInteractSound;
        }

        private void OnDisable()
        {
            _vesselStateTracker.VictoryAudioClipEnabled -= OnPlayInteractSound;
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
