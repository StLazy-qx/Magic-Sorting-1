using Assets.Source.Scripts.Items;
using Assets.Source.Scripts.Player;
using System.Collections.Generic;
using Assets.Source.Scripts.Extensions;
using System.Linq;
using UnityEngine;
using YG;

namespace Assets.Source.Scripts.Storage
{
    public class Store : MonoBehaviour
    {
        [SerializeField] private List<ItemSO> _itemsData;
        [SerializeField] private PlayerEntity _player;
        [SerializeField] private Inventory _inventory;

        private Wallet _playerWallet;

        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            ValidateInitializeArguments();

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
            ValidateInventory(selectedItem, _inventory);

            if (_inventory.HasItem(selectedItem.ID))
                return;

            if (_playerWallet.CanAfford(selectedItem.Price))
            {
                _playerWallet.SpendPoints(selectedItem.Price);
                selectedItem.Buy();
                _inventory.AddItem(selectedItem);
                EquipItem(selectedItem);
            }
        }

        public void EquipItem(Item selectedItem)
        {
            ValidateInventory(selectedItem, _inventory);

            if (_inventory.HasItem(selectedItem.ID) == false)
                return;

            _inventory.EquipItem(selectedItem);
            _inventory.ApplyScin(selectedItem);
        }

        private Item GetItemByID(string id)
        {
            Guard.NotNullOrWhiteSpace(id, nameof(id));

            ItemSO data = _itemsData.FirstOrDefault(item => item.ID == id);

            Guard.NotNull(data, nameof(id));

            Item item = Instantiate(data.Item);

            Guard.NotNull(item, nameof(item));
            item.Initialize(data);

            return item;
        }

        private void LoadPurchasedItems()
        {
            IReadOnlyList<string> savedIDs = YG2.saves.GetPurchasedItems();

            foreach (string itemID in savedIDs)
            {
                Item item = GetItemByID(itemID);

                if (item != null)
                    _inventory.LoadItem(item);
            }

            string equippedID = YG2.saves.EquippedItemID;

            if (string.IsNullOrEmpty(equippedID) == false)
            {
                Item equippedItem = GetItemByID(equippedID);

                if (equippedItem != null)
                    _inventory.SetEquippedItemLoad(equippedItem);
            }
        }

        private void GrantFirstItem()
        {
            int indexFirstSkin = 0;
            ItemSO firstItemData = _itemsData[indexFirstSkin];

            YG2.saves.AddItem(firstItemData.ID);
            YG2.saves.SaveEquippedItem(firstItemData.ID);

            Item firstItem = GetItemByID(firstItemData.ID);

            _inventory.LoadItem(firstItem);
            _inventory.SetEquippedItemLoad(firstItem);
        }

        private void ValidateInitializeArguments()
        {
            Guard.NotNull(_player, nameof(_player));
            Guard.NotNull(_inventory, nameof(_inventory));
            Guard.NotNullOrEmpty(_itemsData, nameof(_itemsData));

            _playerWallet = _player.Wallet;

            Guard.NotNull(_playerWallet, nameof(_playerWallet));
            Guard.IsTrue(_itemsData.All(item => item != null),
                nameof(_itemsData),
                "Items list contains null ItemSO entries.");
        }

        private void ValidateInventory(Item selectedItem, Inventory inventory)
        {
            Guard.NotNull(selectedItem, nameof(selectedItem));
            Guard.NotNull(inventory, nameof(inventory));
            Guard.NotNegative(selectedItem.Price,
                $"{nameof(selectedItem)}.{nameof(selectedItem.Price)}");
        }
    }
}