using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Save_Policies;
using System.Collections.Generic;
using YG;

namespace Assets.Source.Scripts.YG
{
    public class InventorySection : BaseSection
    {
        public InventorySection(SavesYG savesContainer, ISavePolicy savePolicy)
            : base(savesContainer, savePolicy) { }

        public string EquippedItemIdentifier => _savesContainer.EquippedItem;

        public void AddItem(string itemIdentifier)
        {
            Guard.NotNullOrWhiteSpace(itemIdentifier, nameof(itemIdentifier));

            if (_savesContainer.PurchasedItemIdentifiers.Contains(itemIdentifier))
                return;

            _savesContainer.PurchasedItemIdentifiers.Add(itemIdentifier);
            _savePolicy.RequestSave();
        }

        public void SaveEquippedItem(string itemIdentifier)
            => Mutate(
                ref _savesContainer.EquippedItem,
                itemIdentifier,
                validatedItemIdentifier => Guard.NotNullOrWhiteSpace(validatedItemIdentifier, nameof(itemIdentifier)));

        public IReadOnlyList<string> GetPurchasedItems()
            => _savesContainer.PurchasedItemIdentifiers.AsReadOnly();
    }

    //public class InventorySection : BaseSection
    //{
    //    public InventorySection(SavesYG saves) : base(saves) { }

    //    public string EquippedItemID => _saves.EquippedItem;

    //    public void AddItem(string itemID)
    //    {
    //        Guard.NotNullOrWhiteSpace(itemID, nameof(itemID));

    //        if (_saves.ItemIDs.Contains(itemID))
    //            return;

    //        _saves.ItemIDs.Add(itemID);
    //        SaveProgress();
    //    }

    //    public void SaveEquippedItem(string itemID)
    //    {
    //        Guard.NotNullOrWhiteSpace(itemID, nameof(itemID));

    //        _saves.EquippedItem = string.IsNullOrWhiteSpace(itemID)
    //            ? string.Empty
    //            : itemID;

    //        SaveProgress();
    //    }

    //    public IReadOnlyList<string> GetPurchasedItems()
    //    {
    //        return _saves.ItemIDs.AsReadOnly();
    //    }
    //}
}
