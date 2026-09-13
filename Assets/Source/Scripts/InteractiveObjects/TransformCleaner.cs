using Assets.Source.Scripts.Extensions;
using UnityEngine;

namespace Assets.Source.Scripts.InteractiveObjects
{
    class TransformCleaner : MonoBehaviour
    {
        [SerializeField] private Transform _transform;

        public void ClearChildren()
        {
            Guard.NotNull(_transform, nameof(_transform));

            for (int i = _transform.childCount - 1; i >= 0; i--)
            {
                Transform child = _transform.GetChild(i);

                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }
    }
}
