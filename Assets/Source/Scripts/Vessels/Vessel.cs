using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.MagicCells;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Scripts.Vessels
{
    [RequireComponent(typeof(VolumeAggregator))]

    public class Vessel : MonoBehaviour, IColorable, IVesselable
    {
        private const float DeliveryDelay = 1.2f;

        [SerializeField] private Liquid _liquid;
        [SerializeField] private int _maxSize;
        [SerializeField] private int _points;

        private Color _mainColor;
        private VolumeAggregator _aggregator;
        //private MagicCellDelayHandler _deliveryHandler;
        private WaitForSeconds _deliveryWait;

        public event Action<Vector3> Filled;
        public event Action<Vector3, int, Color> RewardIssued;

        public int Count => _maxSize;
        public bool IsActive => gameObject.activeSelf;
        public Color Color => _mainColor;
        public Liquid Liquid => _liquid;
        public bool IsFilled { get; private set; }

        private void Awake()
        {
            ValidateInitializeArguments();

            _aggregator = GetComponent<VolumeAggregator>();

            Guard.NotNull(_aggregator, nameof(_aggregator));

            //_deliveryHandler = new MagicCellDelayHandler(DeliveryDelay);
            _deliveryWait = new WaitForSeconds(DeliveryDelay);

            _aggregator.InitParameters(_maxSize, _liquid);

            IsFilled = false;
        }

        //private void OnDisable()
        //{
        //    _deliveryHandler?.Cancel();
        //}

        //private void OnDestroy()
        //{
        //    _deliveryHandler?.Dispose();
        //}

        public void TakeMagic(MagicCell cell)
        {
            if (cell == null)
                return;

            //_deliveryHandler.TakeMagic(cell, HandleMagicDelivered);

            StartCoroutine(ExecuteAfterDelay());
        }

        public void SetColor(Color color)
            => _mainColor = color;

        private void HandleMagicDelivered()
        {
            _aggregator.GrowUpVolume();

            if (_aggregator.IsFull)
            {
                IsFilled = true;

                RewardIssued?.Invoke(transform.position, 
                    _points, _mainColor);
                Filled?.Invoke(transform.position);
                gameObject.SetActive(false);
            }
        }

        private IEnumerator ExecuteAfterDelay()
        {
            yield return _deliveryWait;

            _aggregator.GrowUpVolume();

            if (_aggregator.IsFull)
            {
                IsFilled = true;

                RewardIssued?.Invoke(transform.position, _points, _mainColor);
                Filled?.Invoke(transform.position);
                gameObject.SetActive(false);
            }
        }

        private void ValidateInitializeArguments()
        {
            Guard.NotNull(_liquid, nameof(_liquid));
            Guard.Positive(_maxSize, nameof(_maxSize));
            Guard.NotNegative(_points, nameof(_points));
        }
    }
}