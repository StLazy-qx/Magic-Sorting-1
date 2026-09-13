using Assets.Source.Scripts.Audio;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.Pool;
using System.Collections;
using UnityEngine;
using System;

namespace Assets.Source.Scripts.ActionsHandlers
{
    public class ActionHandler : MonoBehaviour
    {
        [SerializeField] private SoundPlayer _soundPlayer;
        [SerializeField] private MagicCellRouter _cellRouter;
        [SerializeField] private ParticlePool _particlePool;
        [SerializeField] private float _moveDuration = 1f;

        public event Action SkillUsed;

        private void OnEnable()
        {
            _cellRouter.CellDelivering += OnPlayerAction;
        }

        private void OnDisable()
        {
            _cellRouter.CellDelivering -= OnPlayerAction;
        }

        private void OnPlayerAction(
            Vector3 beginPosition, 
            Vector3 targetPosition, 
            Color color)
        {
            ValidateParameters(beginPosition, targetPosition);
            SkillUsed.Invoke();
            _soundPlayer.PlayInteractSound();
            StartCoroutine(MoveParticle(
                beginPosition, 
                targetPosition, 
                color));
        }

        private IEnumerator MoveParticle(
            Vector3 beginPosition, 
            Vector3 targetPosition, 
            Color color)
        {
            ParticleSystem particle = _particlePool.HandOver();

            if (particle == null)
                yield break;

            SetupParticle(particle, beginPosition, color);

            yield return AnimateParticleMovement(particle, beginPosition, targetPosition);

            particle.Stop();
        }

        private void SetupParticle(
            ParticleSystem particle,
            Vector3 beginPosition,
            Color color)
        {
            particle.transform.position = beginPosition;
            particle.transform.rotation = Quaternion.identity;

            SetParticleColor(particle, color);
            particle.Play();
        }

        private void SetParticleColor(ParticleSystem particle, Color color)
        {
            ParticleSystem.MainModule main = particle.main;
            main.startColor = color;
            ParticleSystem[] childParticles =
                particle.GetComponentsInChildren<ParticleSystem>();

            foreach (ParticleSystem childParticle in childParticles)
            {
                if (childParticle != particle)
                {
                    ParticleSystem.MainModule childMain = childParticle.main;
                    childMain.startColor = color;
                }
            }
        }

        private IEnumerator AnimateParticleMovement(
            ParticleSystem particle, 
            Vector3 beginPosition, 
            Vector3 targetPosition)
        {
            float elapsed = 0f;

            while (elapsed < _moveDuration)
            {
                elapsed += Time.deltaTime;
                float currentTime = Mathf.Clamp01(elapsed / _moveDuration);
                particle.transform.position = Vector3.Lerp
                    (beginPosition, targetPosition, currentTime);

                yield return null;
            }
        }

        private void ValidateParameters(Vector3 beginPosition, Vector3 targetPosition)
        {
            float nullDuration = 0f;

            Guard.NotNull(_soundPlayer, nameof(_soundPlayer));
            Guard.NotNull(_cellRouter, nameof(_cellRouter));
            Guard.NotNull(_particlePool, nameof(_particlePool));
            Guard.IsTrue(_moveDuration > nullDuration, 
                "Move duration must be positive.");
            Guard.IsTrue(beginPosition.IsValid(), nameof(beginPosition), 
                "Position contains NaN or Infinity");
            Guard.IsTrue(targetPosition.IsValid(), nameof(targetPosition), 
                "Position contains NaN or Infinity");
        }
    }
}