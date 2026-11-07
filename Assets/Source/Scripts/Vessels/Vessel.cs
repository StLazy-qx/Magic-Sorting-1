using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.UI.Buttons;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.Vessels
{
    [RequireComponent(typeof(VolumeAggregator))]
    [RequireComponent(typeof(Collider))]
    public class Vessel : MonoBehaviour, IColorable, IVesselable
    {
        private const float DeliveryDelay = 1.2f;

        [SerializeField] private Liquid _liquid;
        [SerializeField] private VesselCloseButton _closeButton;
        [SerializeField] private int _maxSize;
        [SerializeField] private int _points;

        private Color _mainColor;
        private VolumeAggregator _aggregator;
        private MagicCellDelayHandler _deliveryHandler;
        private VesselReward _reward;

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
            _deliveryHandler = new MagicCellDelayHandler(DeliveryDelay);
            _reward = new VesselReward();

            _aggregator.InitParameters(_maxSize, _liquid);

            IsFilled = false;
        }

        private void OnEnable()
        {
            _closeButton.Clicked += OnClose;
        }

        private void OnValidate() 
            => ValidateInitializeArguments();

        private void OnDisable()
        {
            _closeButton.Clicked -= OnClose;

            _deliveryHandler?.Cancel();
        }

        private void OnDestroy() 
            => _deliveryHandler?.Dispose();

        private void OnMouseDown()
        {
            if (IsFilled || IsActive == false)
                return;

            _closeButton?.Show(this);
        }

        public void TakeMagic(MagicCell cell)
        {
            if (cell == null || IsFilled) 
                return;

            _deliveryHandler.TakeMagic(cell, HandleMagicDelivered);
        }

        public void SetColor(Color color) 
            => _mainColor = color;

        public void CompleteVessel()
        {
            //if (IsFilled) 
            //    return;

            IsFilled = true;

            _closeButton?.Hide();

            int reward = _reward.Calculate
                (_points, _maxSize, _aggregator.CurrentVolume);

            RewardIssued?.Invoke(transform.position, reward, _mainColor);

            Debug.Log($"полные очки{reward}");

            Filled?.Invoke(transform.position);
            gameObject.SetActive(false);
        }

        private void OnClose()
        {
            _closeButton?.Hide();

            int reward = _reward.Calculate
                (_points, _maxSize, _aggregator.CurrentVolume);

            RewardIssued?.Invoke(transform.position, reward, _mainColor);

            Debug.Log($"очки по  нќѕ е {reward}");

            Filled?.Invoke(transform.position);
            gameObject.SetActive(false);
        }

        public void ResetState()
        {
            gameObject.SetActive(true);
            _deliveryHandler?.Cancel();
            _closeButton?.Hide();

            IsFilled = false;

            _aggregator?.ResetVolume();
        }

        private void HandleMagicDelivered()
        {
            _aggregator.GrowUpVolume();

            if (_aggregator.IsFull)
                CompleteVessel();
        }

        private void ValidateInitializeArguments()
        {
            Guard.NotNull(_liquid, nameof(_liquid));
            Guard.Positive(_maxSize, nameof(_maxSize));
            Guard.NotNegative(_points, nameof(_points));
        }
    }
}