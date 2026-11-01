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
        private const string ProgressFormat = "{0} / {1}";

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
            Guard.Positive(savedRoundNumber, nameof(savedRoundNumber));

            if (_sequenceProvider == null)
                return;

            SetText(_easyProgressText, FormatProgress(savedRoundNumber, GameMode.Easy));
            SetText(_mediumProgressText, FormatProgress(savedRoundNumber, GameMode.Medium));
            SetText(_hardProgressText, FormatProgress(savedRoundNumber, GameMode.Hard));
        }

        private string FormatProgress(int savedRoundNumber, GameMode mode)
        {
            int current = _sequenceProvider.GetCurrentRoundInMode(savedRoundNumber, mode);
            int total = _sequenceProvider.GetTotalRoundsInMode(mode);

            return string.Format(ProgressFormat, current, total);
        }

        private static void SetText(TextMeshProUGUI target, string text)
        {
            if (target != null)
                target.text = text;
        }
    }
}