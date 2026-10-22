using System.Collections.Generic;
using YG;

namespace Assets.Source.Scripts.Storage
{
    public class StorePersistence
    {
        public IReadOnlyList<string> GetPurchasedItemIds()
            => YG2.saves.GetPurchasedItems();

        public string GetEquippedItemId()
            => YG2.saves.EquippedItemID;

        public void SavePurchasedItem(string id)
            => YG2.saves.AddItem(id);

        public void SaveEquippedItem(string id)
            => YG2.saves.SaveEquippedItem(id);
    }
}