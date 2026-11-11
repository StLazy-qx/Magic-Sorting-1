using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.Vessels;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.Player
{
    [RequireComponent(typeof(Animator))]

    public class MagicianAnimator : MonoBehaviour
    {
        private readonly int AnimationInteract = Animator.StringToHash("Interact");
        private readonly int AnimationChangeClothes = Animator.StringToHash("ChangeClothes");
        private readonly int AnimationWinning = Animator.StringToHash("Winning");

        [SerializeField] private SkinSetter _scinSetter;
        [SerializeField] private ActionHandler _actionHandler;
        [SerializeField] private VesselRoundController _vesselRoundController;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            if (_animator == null)
                throw new ArgumentNullException(nameof(_animator));

            if (_actionHandler == null)
                throw new ArgumentNullException(nameof(_actionHandler));

            if (_vesselRoundController == null)
                throw new ArgumentNullException(nameof(_vesselRoundController));
        }

        private void OnEnable()
        {
            _scinSetter.ItemChanged += OnPlayAnimationChangeClothes;
            _vesselRoundController.RoundOvering += OnPlayAnimationWinning;
            _actionHandler.SkillUsed += OnPlayInteract;
        }

        private void OnDisable()
        {
            _scinSetter.ItemChanged -= OnPlayAnimationChangeClothes;
            _vesselRoundController.RoundOvering -= OnPlayAnimationWinning;
            _actionHandler.SkillUsed -= OnPlayInteract;
        }

        private void OnPlayInteract()
            => _animator.SetTrigger(AnimationInteract);

        private void OnPlayAnimationChangeClothes()
        {
            _animator.SetTrigger(AnimationChangeClothes);
        }

        private void OnPlayAnimationWinning()
            => _animator.SetTrigger(AnimationWinning);
    }
}