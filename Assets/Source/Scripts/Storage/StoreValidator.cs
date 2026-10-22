using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Items;
using Assets.Source.Scripts.Player;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Source.Scripts.Storage
{
    public class StoreValidator
    {
        private readonly PlayerEntity _player;
        private readonly Inventory _inventory;
        private readonly IReadOnlyList<ItemSO> _itemsData;

        public StoreValidator(
            PlayerEntity player,
            Inventory inventory,
            IReadOnlyList<ItemSO> itemsData)
        {
            _player = player;
            _inventory = inventory;
            _itemsData = itemsData;
        }
        
        public Wallet ValidateInitializeAndGetWallet()
        {
            Guard.NotNull(_player, nameof(_player));
            Guard.NotNull(_inventory, nameof(_inventory));
            Guard.NotNullOrEmpty(_itemsData, nameof(_itemsData));
            Guard.NotNull(_player.Wallet, nameof(_player.Wallet));
            Guard.IsTrue(
                _itemsData.All(item => item != null),
                nameof(_itemsData),
                "Items list contains null ItemSO entries.");

            return _player.Wallet;
        }

        public void ValidateItem(Item item)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(_inventory, nameof(_inventory));
            Guard.NotNegative(item.Price, $"{nameof(item)}.{nameof(item.Price)}");
        }
    }
}