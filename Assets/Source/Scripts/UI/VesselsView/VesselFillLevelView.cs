using UnityEngine;
using TMPro;
using Assets.Source.Scripts.Vessels;
using System;

namespace Assets.Source.Scripts.UI.VesselsView
{
    public class VesselFillLevelView : MonoBehaviour
    {
        [SerializeField] private VolumeAggregator _vessel;
        [SerializeField] private TMP_Text _countText;

        private void Awake()
        {
            ValidateInitializeArguments();
        }

        private void OnEnable()
        {
            _vessel.SizeChanged += OnFillView;
        }

        private void OnDisable()
        {
            _vessel.SizeChanged -= OnFillView;
        }

        private void OnFillView(int value)
        {
            _countText.text = value.ToString();
        }

        private void ValidateInitializeArguments()
        {
            if (_vessel == null)
            {
                throw new NullReferenceException(
                    "Volume Aggregator reference is missing in VesselFillLevelView.");
            }

            if (_countText == null)
            {
                throw new NullReferenceException(
                    "TMP_Text reference is missing in VesselFillLevelView.");
            }
        }
    }
}