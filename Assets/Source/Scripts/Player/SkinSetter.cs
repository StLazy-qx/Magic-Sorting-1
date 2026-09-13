using Assets.Source.Scripts.Items;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.Player
{
    public class SkinSetter : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer _meshRenderer;

        private Material _materialInstance;

        public event Action ItemChanged;

        private void Start()
        {
            if (_meshRenderer == null)
            {
                throw new ArgumentNullException(nameof(_meshRenderer),
                    "SkinnedMeshRenderer must be assigned");
            }

            _materialInstance = _meshRenderer.material;
        }

        public void ApplyItem(Item item)
        {
            if (item == null || item.Texture == null)
                return;

            if (_materialInstance == null)
            {
                throw new InvalidOperationException(
                    "Material instance is not initialized");
            }

            _materialInstance.SetTexture("_MainTex", item.Texture);
        }

        public void OnShowItemTexture(Texture texture)
        {
            if (texture == null)
                return;

            if (_materialInstance == null)
            {
                throw new InvalidOperationException(
                    "Material instance is not initialized");
            }

            _materialInstance.SetTexture("_MainTex", texture);
        }
    }
}