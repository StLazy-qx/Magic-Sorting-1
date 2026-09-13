using Assets.Source.Scripts.Extensions;
using System.Collections.Generic;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class InventorySection
    {
        private readonly SavesYG _saves;

        public InventorySection(SavesYG saves)
        {
            _saves = saves;
        }

        public string EquippedItemID => _saves.EquippedItem;

        public void AddItem(string itemID)
        {
            Guard.NotNullOrWhiteSpace(itemID, nameof(itemID));

            if (_saves.ItemIDs.Contains(itemID))
                return;

            _saves.ItemIDs.Add(itemID);
            SaveProgress();
        }

        public void SaveEquippedItem(string itemID)
        {
            Guard.NotNullOrWhiteSpace(itemID, nameof(itemID));

            _saves.EquippedItem = string.IsNullOrWhiteSpace(itemID)
                ? string.Empty
                : itemID;

            SaveProgress();
        }

        public IReadOnlyList<string> GetPurchasedItems()
        {
            return _saves.ItemIDs.AsReadOnly();
        }

        public void SaveProgress()
        {
            YG2.SaveProgress();
        }
    }
}
