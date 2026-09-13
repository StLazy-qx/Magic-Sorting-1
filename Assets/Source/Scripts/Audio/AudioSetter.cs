using System;
using UnityEngine;

namespace Assets.Source.Scripts.Audio
{
    public class AudioSetter : MonoBehaviour
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField] private AudioSource _audioSource;

        private void Awake()
        {
            ValidateInitializeArguments();

            _audioSource = GetComponent<AudioSource>();

            if (_audioSource == null)
            {
                throw new ArgumentNullException
                    (nameof(_audioSource), "AudioSource cannot be null");
            }

            _audioSource.clip = _clip;
        }

        private void Start()
        {
            PlayMusic();
        }

        public void PlayMusic()
            => _audioSource.Play();

        private void ValidateInitializeArguments()
        {
            if (_clip == null)
            {
                throw new ArgumentNullException
                    (nameof(_clip), "AudioClips cannot be null");
            }

            if (_audioSource == null)
            {
                throw new ArgumentNullException
                    (nameof(_audioSource), "AudioClips cannot be null");
            }
        }
    }
}