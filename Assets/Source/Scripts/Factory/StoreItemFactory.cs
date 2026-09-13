using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.Items;
using Assets.Source.Scripts.Storage;
using Assets.Source.Scripts.UI.StoreView;
using System.Collections.Generic;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Pool;
using Assets.Source.Scripts.Enums;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace Assets.Source.Scripts.Factory
{
    public class StoreItemFactory : Factory<Button>
    {
        [SerializeField] private Store _store;
        [SerializeField] private SkinSetter _modelSkin;
        [SerializeField] private Inventory _inventory;
        [SerializeField] private ItemViewPool _itemViewPool;

        private ItemSelectionHandler _itemPresenter;
        private Transform _contentTransform;

        public event Action<Button> Created;

        public void Initialize(
            Transform contentTransform, 
            ItemSelectionHandler itemPresenter)
        {
            _contentTransform = contentTransform;
            _itemPresenter = itemPresenter;

            _itemViewPool.SetContainer(_contentTransform);
        }

        protected override void BuildObjects()
        {
            ValidateBuildRequirements();
            ClearList();
            _itemViewPool.Clear();

            IReadOnlyList<ItemSO> items = _store.GetItemsSO();

            foreach (ItemSO itemData in items)
            {
                Button button = Instantiate(Prefab, _contentTransform);
                Item item = button.GetComponent<Item>();
                NewItemView itemView = item.GetComponent<NewItemView>();

                OnPushItemPresenter(itemView);
                Guard.NotNull(item, nameof(item));
                Guard.NotNull(itemView, nameof(itemView));
                item.Initialize(itemData);
                itemView.Initialize(itemData);
                _itemViewPool.Add(itemView);
                Add(button);
                Created?.Invoke(button);
            }

            NotifyObjectsChanged();
        }

        protected override void OnDifficultyChanged(DifficultyLevel level) {}

        private void OnPushItemPresenter(NewItemView newItemView)
        {
            newItemView.OnClicked += _itemPresenter.OnSelectShowedItem;
            newItemView.OnItemSelected += _modelSkin.OnShowItemTexture;
        }

        private void ValidateBuildRequirements()
        {
            Guard.NotNull(_store, nameof(_store));
            Guard.NotNull(Prefab, nameof(Prefab));
            Guard.NotNull(_itemPresenter, nameof(_itemPresenter));
            Guard.NotNull(_contentTransform, nameof(_contentTransform));
            Guard.NotNull(_itemViewPool, nameof(_itemViewPool));
        }
    }
}