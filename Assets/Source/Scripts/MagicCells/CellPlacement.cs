using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.MagicCells;
using UnityEngine;

namespace Assets.Source.Scripts.MagicCells
{
    public class CellPlacement
    {
        private const float RotationX = 90f;

        public Quaternion GetRotation()
        {
            return Quaternion.Euler(RotationX, 0f, 0f);
        }

        public Vector3 GetPosition(Transform storagePoint)
        {
            Guard.NotNull(storagePoint, nameof(storagePoint));
            return storagePoint.position;
        }

        public MagicCell Instantiate(MagicCell prefab, Transform storagePoint, bool isActive = true)
        {
            Guard.NotNull(prefab, nameof(prefab));
            Guard.NotNull(storagePoint, nameof(storagePoint));

            MagicCell cell = Object.Instantiate(prefab, GetPosition(storagePoint), GetRotation());
            cell.gameObject.SetActive(isActive);
            return cell;
        }
    }
}
