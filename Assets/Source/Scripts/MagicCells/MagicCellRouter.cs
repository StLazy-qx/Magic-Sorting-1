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
        [SerializeField] private WaitingPoint _waitingPoint;
        [SerializeField] private MagicMixer _magicMixer;
        [SerializeField] private DelayDispatcher _delayDispatcher;

        private IReadOnlyList<Vessel> _vessels;

        public event Action<Vector3, Vector3, Color> CellDelivering;
        public event Action CellDeparturing;
        public event Action WaitingPointUsing;
        public event Action CellSentToMixer;

        private void Awake()
        {
            Guard.NotNull(_waitingPoint, nameof(_waitingPoint));
            Guard.NotNull(_delayDispatcher, nameof(_delayDispatcher));
            Guard.NotNull(_magicMixer, nameof(_magicMixer));
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

        public bool IsCheckCellColor(Color color)
        {
            return FindVesselByColor(color) != null
                || _waitingPoint.IsFreePlace;
        }

        public bool CanAcceptCellMixer(MagicCell cell)
        {
            return _magicMixer != null
                && cell != null
                && _magicMixer.CanAcceptCellColor(cell.Color);
        }

        public bool DeliverMagicCellMixer(MagicCell cell)
        {
            Guard.NotNull(cell, nameof(cell));
            Guard.NotNull(_magicMixer, nameof(_magicMixer));

            if (_magicMixer.CanAcceptCellColor(cell.Color) == false)
                return false;

            CellDeparturing?.Invoke();

            Deliver(cell, _magicMixer.transform);
            _magicMixer.AcceptCell(cell);
            CellSentToMixer?.Invoke();

            return true;
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