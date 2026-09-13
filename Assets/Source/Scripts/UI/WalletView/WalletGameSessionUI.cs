using UnityEngine;
using TMPro;
using Zenject;
using Assets.Source.Scripts.Player;
using System;

namespace Assets.Source.Scripts.UI.WalletView
{
    public class WalletGameSessionUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _moneyText;

        private Wallet _wallet;

        private void Awake()
        {
            if (_moneyText == null)
            {
                throw new NullReferenceException(
                    "MoneyText reference is missing in WalletGameSessionUI.");
            }
        }

        private void OnEnable()
        {
            _wallet.TotalScoreChanged += OnCoinView;

            OnCoinView(_wallet.DisplayScore);
        }

        private void OnDisable()
        {
            _wallet.TotalScoreChanged -= OnCoinView;
        }

        [Inject]
        private void Construct(Wallet wallet)
        {
            _wallet = wallet ?? throw new ArgumentNullException(nameof(wallet),
                "Injected Wallet cannot be null in WalletGameSessionUI.");
        }

        private void OnCoinView(int value)
        {
            _moneyText.text = value.ToString();
        }
    }
}