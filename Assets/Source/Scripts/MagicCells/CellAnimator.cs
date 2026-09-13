using UnityEngine;

namespace Assets.Source.Scripts.MagicCells
{
    class CellAnimator : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private MagicCell _magicCell;

        private void Start()
        {
            SetColor();
        }

        private void SetColor()
        {
            ParticleSystem[] childParticles = _particle.GetComponentsInChildren<ParticleSystem>();

            foreach (ParticleSystem childParticle in childParticles)
            {
                ParticleSystem.MainModule childMain = childParticle.main;
                childMain.startColor = _magicCell.Color;
            }
        }
    }
}
