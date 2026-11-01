using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.Player;
using TMPro;
using UnityEngine;
using YG;

namespace Assets.Source.Scripts.Game_Progress
{
    public class RoundProgressDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _easyProgressText;
        [SerializeField] private TextMeshProUGUI _mediumProgressText;
        [SerializeField] private TextMeshProUGUI _hardProgressText;

        private RoundSequenceProvider _sequenceProvider;

        public void Initialize(RoundSequenceProvider sequenceProvider)
        {
            Guard.NotNull(sequenceProvider, nameof(sequenceProvider));

            _sequenceProvider = sequenceProvider;
            RefreshFromSaves();
        }

        public void RefreshFromSaves()
        {
            if (_sequenceProvider == null)
                return;

            Refresh(YG2.saves.GetRoundNumber());
        }

        public void Refresh(int savedRoundNumber)
        {
            if (_sequenceProvider == null)
                return;

            SetText(_easyProgressText,   _sequenceProvider.GetProgressText(savedRoundNumber, GameMode.Easy));
            SetText(_mediumProgressText, _sequenceProvider.GetProgressText(savedRoundNumber, GameMode.Medium));
            SetText(_hardProgressText,   _sequenceProvider.GetProgressText(savedRoundNumber, GameMode.Hard));
        }

        private static void SetText(TextMeshProUGUI target, string text)
        {
            if (target != null)
                target.text = text;
        }
    }
}