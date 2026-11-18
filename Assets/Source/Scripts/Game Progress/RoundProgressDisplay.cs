using Assets.Source.Scripts.Player;
using TMPro;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.Game_Progress
{
    public class RoundProgressDisplay : MonoBehaviour
    {
        private const string ProgressFormat = "{0} / {1}";

        [SerializeField] private TextMeshProUGUI _easyProgressText;
        [SerializeField] private TextMeshProUGUI _mediumProgressText;
        [SerializeField] private TextMeshProUGUI _hardProgressText;
        [SerializeField] private TextMeshProUGUI _freeRoundNumberText;

        [Inject] private PlayerProgress _playerProgress;

        public void Initialize()
        {
            RefreshFromSaves();
        }

        public void RefreshFromSaves()
        {
            if (_playerProgress == null)
                return;

            SetText(_easyProgressText, FormatProgress(GameMode.Easy));
            SetText(_mediumProgressText, FormatProgress(GameMode.Medium));
            SetText(_hardProgressText, FormatProgress(GameMode.Hard));
            SetText(_freeRoundNumberText, _playerProgress.CurrentRoundNumber.ToString());
        }

        private string FormatProgress(GameMode mode)
        {
            if (_playerProgress.IsModeAvailable(mode) == false)
                return string.Empty;

            int completed = _playerProgress.GetCompletedRoundsCount(mode);
            int total = _playerProgress.GetTotalRoundsCount(mode);

            return string.Format(ProgressFormat, completed, total);
        }

        private static void SetText(TextMeshProUGUI target, string text)
        {
            if (target != null)
                target.text = text;
        }
    }
}