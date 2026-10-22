using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Items;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Scripts.Storage
{
    public class ItemBuilder
    {
        private readonly IReadOnlyList<ItemSO> _itemsData;

        public ItemBuilder(IReadOnlyList<ItemSO> itemsData)
        {
            Guard.NotNullOrEmpty(itemsData, nameof(itemsData));
            
            _itemsData = itemsData;
        }

        public Item Build(string id)
        {
            Guard.NotNullOrWhiteSpace(id, nameof(id));

            ItemSO data = _itemsData.FirstOrDefault(item => item.ID == id);
            
            Guard.NotNull(data, nameof(id));

            Item item = Object.Instantiate(data.Item);
            
            Guard.NotNull(item, nameof(item));
            item.Initialize(data);
            
            return item;
        }
    }
}