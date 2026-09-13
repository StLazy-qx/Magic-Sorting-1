using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Items;
using Assets.Source.Scripts.Storage;
using Assets.Source.Scripts.UI.StoreView;
using Assets.Source.Scripts.Extensions;
using System.Collections.Generic;
using System;
using UnityEngine;
using YG;
using System.Linq;

namespace Assets.Source.Scripts.Player
{
    public class Inventory : MonoBehaviour, IObjectInitilizable
    {
        [SerializeField] private Store _store;
        [SerializeField] private SkinSetter _scinSetter;

        private Item _equippedItem;
        private List<Item> _items = new List<Item>();

        public event Action<NewItemView> ItemBuyed;
        public event Action Initialized;

        public bool IsEmpty => _items.Count == 0;
        public bool IsInitialized { get; private set; }
        public Item EquippedItem => _equippedItem;

        public void Initialize()
        {
            Guard.NotNull(_store, nameof(_store));
            Guard.NotNull(_scinSetter, nameof(_scinSetter));

            IsInitialized = true;

            Initialized?.Invoke();
        }

        public bool HasItem(string itemID)
        {
            Guard.NotNullOrWhiteSpace(itemID, nameof(itemID));

            return _items.Any(item => item.ID == itemID);
        }

        public void AddItem(Item item)
        {
            ValidateItem(item);

            if (HasItem(item.ID))
                return;

            _items.Add(item);
            YG2.saves.AddItem(item.ID);

            NewItemView newItemView = item.GetComponent<NewItemView>();

            if(newItemView != null)
                ItemBuyed?.Invoke(newItemView);
        }

        public void LoadItem(Item item)
        {
            ValidateItem(item);

            if (HasItem(item.ID)) 
                return;

            _items.Add(item);
        }

        public void EquipItem(Item item)
        {
            ValidateItem(item);
            item.Equip();

            if (_equippedItem != null)
                _equippedItem.UnEquip();

            _equippedItem = item;
            
            YG2.saves.SaveEquippedItem(item.ID);
        }

        public void SetEquippedItemLoad(Item item)
        {
            if (item == null) 
                return;

            _equippedItem = item;

            item.Equip();
        }

        public void SetScin()
            => _scinSetter.ApplyItem(_equippedItem);

        public void ApplyScin(Item item)
            => _scinSetter.ApplyItem(item);

        private void ValidateItem(Item item)
            => Guard.NotNull(item, nameof(item));
    }
}