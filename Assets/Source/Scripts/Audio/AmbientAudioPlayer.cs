using System;
using System.Collections;
using UnityEngine;
using System.Linq;

namespace Assets.Source.Scripts.Audio
{
    public class AmbientAudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioClip[] _clips;

        private int _currentIndex;
        private AudioClip[] _shuffledClips;

        private void Awake()
        {
            ValidateInitializeArguments();
        }

        private void Start()
        {
            ShuffleClips();
            StartCoroutine(PlayClipsInSequence());
        }

        private void ShuffleClips()
        {
            _shuffledClips = _clips.OrderBy(
                clip => UnityEngine.Random.value).ToArray();
            _currentIndex = 0;
        }

        private IEnumerator PlayClipsInSequence()
        {
            while (isActiveAndEnabled)
            {
                if (_currentIndex >= _shuffledClips.Length)
                    ShuffleClips();

                AudioClip currentClip = _shuffledClips[_currentIndex];
                _source.clip = currentClip;

                _source.Play();

                yield return new WaitForSeconds(currentClip.length);

                _currentIndex++;
            }
        }

        private void ValidateInitializeArguments()
        {
            if (_source == null)
            {
                throw new ArgumentNullException
                    (nameof(_source), "AudioSource cannot be null");
            }

            if (_clips == null)
            {
                throw new ArgumentNullException
                    (nameof(_clips), "AudioClips array cannot be null");
            }

            if (_clips.Length == 0)
                return;
        }
    }
}
