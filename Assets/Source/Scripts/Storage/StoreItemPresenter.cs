using Assets.Source.Scripts.Items;
using Assets.Source.Scripts.UI.StoreView;
using System;
using UnityEngine;
using UnityEngine.UI;


//не используется
namespace Assets.Source.Scripts.Storage
{
    class StoreItemPresenter : MonoBehaviour
    {
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _equipButton;
        [SerializeField] private ItemView _itemView;

        private Store _store;
        private Item _item;

        private void OnDestroy()
        {
            _buyButton.onClick.RemoveListener(OnBuyButtonClicked);
            _equipButton.onClick.RemoveListener(OnItemEquipped);
        }

        public void Initialize(Item item, Store store)
        {
            _store = store 
                ?? throw new ArgumentNullException(nameof(store));
            _item = item
                ?? throw new ArgumentNullException(nameof(item));

            _buyButton.onClick.AddListener(OnBuyButtonClicked);
            _equipButton.onClick.AddListener(OnItemEquipped);
        }

        private void OnBuyButtonClicked()
        {
            _store.BuyItem(_item);

            if (_item.IsBought)
                _itemView.HideBuyButton();
        }

        private void OnItemEquipped()
        {
            _store.EquipItem(_item);

            if (_item.IsBought)
                _itemView.SetSelectedState();
        }
    }
}
