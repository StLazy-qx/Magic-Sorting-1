using System.Collections.Generic;
using YG;

namespace Assets.Source.Scripts.Storage
{
    public class StorePersistence
    {
        public IReadOnlyList<string> GetPurchasedItemIds()
            => YG2.saves.Inventory.GetPurchasedItems();

        public string GetEquippedItemId()
            => YG2.saves.EquippedItemIdentifier;

        public void SavePurchasedItem(string id)
            => YG2.saves.Inventory.AddItem(id);

        public void SaveEquippedItem(string id)
            => YG2.saves.Inventory.SaveEquippedItem(id);
    }
}