using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.Vessels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Scripts.MagicCells
{
    public class MagicCellRouter : MonoBehaviour
    {
        private const float DeliveryDelay = 1.2f;

        [SerializeField] private WaitingPoint _waitingPoint;
        [SerializeField] private MagicMixer _magicMixer;
        [SerializeField] private DelayDispatcher _delayDispatcher;

        private WaitForSeconds _deliveryWait;
        private IReadOnlyList<Vessel> _vessels;

        public event Action<Vector3, Vector3, Color> CellDelivering;
        public event Action CellDeparturing;
        public event Action WaitingPointUsing;

        private void Awake()
        {
            Guard.NotNull(_waitingPoint, nameof(_waitingPoint));
            Guard.NotNull(_delayDispatcher, nameof(_delayDispatcher));
            Guard.NotNull(_magicMixer, nameof(_magicMixer));

            _deliveryWait = new WaitForSeconds(DeliveryDelay);
        }

        public void Initialize(IReadOnlyList<Vessel> vessels)
        {
            Guard.NotNull(vessels, nameof(vessels));
            Guard.IsTrue(vessels.Count > 0, nameof(vessels),
                "[MagicCellRouter] ������ ����� �� ����� ���� ������.");

            _vessels = vessels;
        }

        public void DeliverMagicCell(MagicCell cell)
        {
            Guard.NotNull(cell, nameof(cell));
            CellDeparturing?.Invoke();

            Vessel targetVessel = FindVesselByColor(cell.Color);

            if (targetVessel != null)
            {
                Deliver(cell, targetVessel.transform);
                targetVessel.TakeMagic(cell);
            }
            else if (_waitingPoint.IsFreePlace)
            {
                WaitingPointUsing?.Invoke();
                Deliver(cell, _waitingPoint.transform);
                _waitingPoint.AcceptStorageCell(cell);
                _waitingPoint.ShowWaitingCellWithDelay();
            }
        }

        public void DeliverMagicCellToMixer(MagicCell cell)
        {
            Guard.NotNull(cell, nameof(cell));
            Guard.NotNull(_magicMixer, nameof(_magicMixer));

            if (_magicMixer.CanAcceptCell == false)
                return;

            CellDeparturing?.Invoke();

            Deliver(cell, _magicMixer.transform);
            _magicMixer.AcceptCell(cell);
        }

        public bool IsCheckCellColor(Color color)
        {
            return FindVesselByColor(color) != null
                || _waitingPoint.IsFreePlace;
        }

        public bool CanAcceptCellToMixer()
        {
            return _magicMixer != null && _magicMixer.CanAcceptCell;
        }

        private void Deliver(MagicCell cell, Transform target)
        {
            float differenceAxisY = 0.5f;
            Vector3 departurePositionCell = new Vector3(
                    cell.transform.position.x,
                    cell.transform.position.y + differenceAxisY,
                    cell.transform.position.z);

            CellDelivering?.Invoke(
                departurePositionCell,
                target.position,
                cell.Color);
        }

        private Vessel FindVesselByColor(Color color)
        {
            return _vessels.FirstOrDefault(
                vessel => vessel.IsActive
                && vessel.Color == color);
        }
    }
}