using System;
using UnityEngine;

namespace Assets.Source.Scripts.Audio
{
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _interactClips;

        private bool _isPlaying;

        public void PlayInteractSound()
        {
            int beginIndex = 0;

            if (_audioSource == null)
            {
                throw new ArgumentNullException
                    (nameof(_audioSource), "AudioSource cannot be null");
            }

            if (_interactClips == null || _interactClips.Length == 0)
                return;

            if (_isPlaying)
                return;

            int randomIndex = UnityEngine.Random.Range(beginIndex, _interactClips.Length);
            AudioClip clip = _interactClips[randomIndex];

            SetRandomPitch();
            _audioSource.PlayOneShot(clip);

            _audioSource.pitch = 1f;
        }

        private void SetRandomPitch()
        {
            _audioSource.pitch = UnityEngine.Random.Range(0.4f, 0.7f);
        }
    }
}