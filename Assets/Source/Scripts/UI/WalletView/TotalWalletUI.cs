using UnityEngine;
using TMPro;
using Zenject;
using Assets.Source.Scripts.Player;
using System;

namespace Assets.Source.Scripts.UI.WalletView
{
    public class TotalWalletUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;

        private Wallet _wallet;

        private void Awake()
        {
            if (_scoreText == null)
            {
                throw new NullReferenceException(
                    "ScoreText reference is missing in TotalWalletUI.");
            }
        }

        private void OnEnable()
        {
            _wallet.TotalScoreChanged += OnCoinView;

            OnCoinView(_wallet.TotalScore);
        }

        private void OnDisable()
        {
            _wallet.TotalScoreChanged -= OnCoinView;
        }

        [Inject]
        private void Construct(Wallet wallet)
        {
            _wallet = wallet ?? throw new ArgumentNullException(nameof(wallet),
                "Injected Wallet cannot be null in TotalWalletUI.");
        }

        private void OnCoinView(int value)
        {
            _scoreText.text = value.ToString();
        }
    }
}