using Assets.Source.Scripts.EntryPoint;
using UnityEngine;
using Zenject;
using System;

namespace Assets.Source.Scripts.Player
{
    public class PlayerEntity : MonoBehaviour, IObjectInitilizable
    {
        private Wallet _wallet;

        public Wallet Wallet => _wallet;

        public bool IsInitialized { get; private set; }

        [Inject]
        public void Construct(Wallet walletl)
        {
            _wallet = walletl ??
                throw new ArgumentNullException(nameof(walletl),
                "[Player] Wallet не может быть null");
        }

        public void Initialize()
        {
            if (_wallet == null)
                throw new ArgumentNullException(nameof(_wallet));

            IsInitialized = true;
        }
    }
}