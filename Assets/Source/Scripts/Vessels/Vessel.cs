using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.UI.StoreView;
using System;
using UnityEngine;

namespace Assets.Source.Scripts.Vessels
{
    [RequireComponent(typeof(VolumeAggregator))]
    public class Vessel : MonoBehaviour, IColorable, IVesselable, IInteractable
    {
        private const float DeliveryDelay = 1.2f;

        [SerializeField] private Liquid _liquid;
        [SerializeField] private ScoreToast _scoreToast;
        [SerializeField] private int _maxSize;
        [SerializeField] private int _points;

        private Color _mainColor;
        private VolumeAggregator _aggregator;
        private MagicCellDelayHandler _deliveryHandler;

        public event Action<Vector3> Filled;
        public event Action<Vector3, int, Color> RewardIssued;
        public event Action Interacted;

        public int Count => _maxSize;
        public int Capacity => _maxSize;
        public bool IsActive => gameObject.activeSelf;
        public Color Color => _mainColor;
        public Liquid Liquid => _liquid;
        public bool IsFilled { get; private set; }

        private void Awake()
        {
            ValidateInitializeArguments();

            _aggregator = GetComponent<VolumeAggregator>();
            _deliveryHandler = new MagicCellDelayHandler(DeliveryDelay);

            _aggregator.InitParameters(_maxSize, _liquid);

            IsFilled = false;
        }

        private void OnValidate()
            => ValidateInitializeArguments();

        private void OnDisable()
            => _deliveryHandler.Cancel();

        private void OnDestroy()
            => _deliveryHandler.Dispose();

        private void OnMouseDown()
            => _scoreToast.Show();

        public void OnClick()
        {
            if (IsFilled || IsActive == false)
                return;

            Interacted?.Invoke();
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
            if (IsFilled)
                return;

            IsFilled = true;

            int reward = VesselReward.Calculate(_points, _maxSize, _aggregator.CurrentVolume);

            RewardIssued?.Invoke(transform.position, reward, _mainColor);
            Filled?.Invoke(transform.position);

            gameObject.SetActive(false);
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