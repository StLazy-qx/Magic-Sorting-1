using System;
using UnityEngine;

namespace Assets.Source.Scripts.Items
{
    public class Item : MonoBehaviour
    {
        private string _id;
        private Texture _texture;
        private int _price;

        public string ID => _id;
        public int Price => _price;
        public Texture Texture => _texture;
        public bool IsBought { get; private set; }
        public bool IsEquipped { get; private set; }

        public void Initialize(ItemSO itemData)
        {
            ValidateObjects(itemData);

            _id = itemData.ID;
            _price = itemData.Price;
            _texture = itemData.Skin;
        }

        public void Buy()
        {
            if (IsBought)
                return;

            IsBought = true;
        }

        public void Equip()
        {
            if (IsBought == false)
                return;

            IsEquipped = true;
        }

        public void UnEquip()
        {
            if (IsEquipped == false)
                return;

            IsEquipped = false;
        }

        private void ValidateObjects(ItemSO itemData)
        {
            if (itemData.Price < 0)
            {
                throw new ArgumentException(
                    "Price must be positive", nameof(itemData));
            }

            if (string.IsNullOrWhiteSpace(itemData.ID))
            {
                throw new ArgumentException(
                    "ID cannot be null or empty", nameof(itemData));
            }
        }
    }
}