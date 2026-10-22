using Assets.Source.Scripts.Items;
using Assets.Source.Scripts.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Storage
{
    public class Store : MonoBehaviour
    {
        [SerializeField] private List<ItemSO> _itemsData;
        [SerializeField] private PlayerEntity _player;
        [SerializeField] private Inventory _inventory;

        private Wallet _playerWallet;
        private ItemBuilder _itemBuilder;
        private StorePersistence _persistence;
        private StoreValidator _validator;

        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            _validator = new StoreValidator(_player, _inventory, _itemsData);
            _playerWallet = _validator.ValidateInitializeAndGetWallet();
            _itemBuilder = new ItemBuilder(_itemsData);
            _persistence = new StorePersistence();

            _inventory.Initialize();
            LoadPurchasedItems();

            if (_inventory.IsEmpty)
                GrantFirstItem();

            IsInitialized = true;
        }

        public IReadOnlyList<ItemSO> GetItemsSO()
            => _itemsData.AsReadOnly();

        public void BuyItem(Item selectedItem)
        {
            _validator.ValidateItem(selectedItem);

            if (_inventory.HasItem(selectedItem.ID))
                return;

            if (_playerWallet.CanAfford(selectedItem.Price) == false)
                return;

            _playerWallet.SpendPoints(selectedItem.Price);
            selectedItem.Buy();
            _inventory.AddItem(selectedItem);
            _persistence.SavePurchasedItem(selectedItem.ID);
            EquipItem(selectedItem);
        }

        public void EquipItem(Item selectedItem)
        {
            _validator.ValidateItem(selectedItem);

            if (_inventory.HasItem(selectedItem.ID) == false)
                return;

            _inventory.EquipItem(selectedItem);
            _inventory.ApplyScin(selectedItem);
            _persistence.SaveEquippedItem(selectedItem.ID);
        }

        private void LoadPurchasedItems()
        {
            IReadOnlyList<string> savedIDs = _persistence.GetPurchasedItemIds();

            foreach (string itemID in savedIDs)
            {
                Item item = _itemBuilder.Build(itemID);
                
                _inventory.LoadItem(item);
            }

            string equippedID = _persistence.GetEquippedItemId();
            
            if (string.IsNullOrEmpty(equippedID))
                return;

            Item equippedItem = _itemBuilder.Build(equippedID);
            
            _inventory.SetEquippedItemLoad(equippedItem);
        }

        private void GrantFirstItem()
        {
            const int firstItemIndex = 0;
            ItemSO firstItemData = _itemsData[firstItemIndex];

            _persistence.SavePurchasedItem(firstItemData.ID);
            _persistence.SaveEquippedItem(firstItemData.ID);

            Item firstItem = _itemBuilder.Build(firstItemData.ID);
            
            _inventory.LoadItem(firstItem);
            _inventory.SetEquippedItemLoad(firstItem);
        }
    }
}