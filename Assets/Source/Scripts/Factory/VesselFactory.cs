using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.Vessels;
using Assets.Source.Scripts.Pool;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Source.Scripts.Extensions;

namespace Assets.Source.Scripts.Factory
{
    public class VesselFactory : Factory<Vessel>
    {
        [SerializeField] private VesselStateTracker _gameFullingBehaviour;
        [SerializeField] private VesselPool _vesselPool;
        [SerializeField] private MagicCellRouter _distributerMagicCell;

        private ColorRandomizer _colorRandomizer;

        public bool IsReady { get; private set; }

        protected override void OnDestroy()
        {
            foreach (var vesell in Objects)
            {
                if (vesell != null)
                    vesell.Filled -= OnPutRemainingVessel;
            }

            base.OnDestroy();
        }

        public void InitRandomizer(ColorRandomizer colorRandomizer)
        {
            Guard.NotNull(colorRandomizer, nameof(colorRandomizer));

            _colorRandomizer = colorRandomizer;
        }

        protected override void BuildObjects()
        {
            ValidateBuildRequirements();
            ClearList();

            DifficultySettings settings = DifficultyDatabase.
                GetSettings(DifficultyState.CurrentDifficulty);

            if (settings.VesselsCount <= 0)
                throw new InvalidOperationException("Vessels count must be > 0");

            for (int i = 0; i < settings.VesselsCount; i++)
            {
                Vessel vessel = Instantiate(Prefab);
                vessel.Filled += OnPutRemainingVessel;

                vessel.gameObject.SetActive(false);
                Add(vessel);
            }

            AssignColorsCount(CurrentSettings.ColorsCount);
            _gameFullingBehaviour.SetVesselsList(Objects);
            ActivateVessels();

            IsReady = true;
        }

        private void OnPutRemainingVessel(Vector3 position)
        {
            Vessel remainingVessel = Objects.FirstOrDefault(
                vessel => vessel.IsFilled == false && 
                vessel.gameObject.activeSelf == false);

            if (remainingVessel == null)
                return;

            remainingVessel.transform.position = position;

            remainingVessel.gameObject.SetActive(true);
        }

        private void ActivateVessels()
        {
            int index = 0;

            foreach (var vessel in Objects)
            {
                if (index >= SpawnPoints.Length)
                    break;

                if (vessel.IsFilled == false && vessel.gameObject.activeSelf == false)
                {
                    vessel.transform.position = SpawnPoints[index].position;

                    vessel.gameObject.SetActive(true);

                    index++;
                }
            }
        }

        private void AssignColorsCount(int colorsCount)
        {
            Guard.Positive(colorsCount, nameof(colorsCount));

            List<Color> palette = new List<Color>(_colorRandomizer.BeginColors);

            palette.AddRange(_colorRandomizer.RemainingColors);

            for (int i = 0; i < Objects.Count; i++)
            {
                Color color = palette[i % palette.Count];

                AssignColor(Objects[i], color);
            }
        }

        private void AssignColor(Vessel vessel, Color color)
        {
            Guard.NotNull(vessel, nameof(vessel));

            bool hasMarker = vessel.TryGetComponent(out ColorMarker colorMarker);

            Guard.IsTrue(hasMarker,
                $"Vessel does not have required {nameof(ColorMarker)} component");

            colorMarker.Initialize(color);
        }

        private void ValidateBuildRequirements()
        {
            Guard.NotNull(_gameFullingBehaviour, nameof(_gameFullingBehaviour));
            Guard.NotNull(_distributerMagicCell, nameof(_distributerMagicCell));
            Guard.NotNull(_colorRandomizer, nameof(_colorRandomizer));
        }
    }
}