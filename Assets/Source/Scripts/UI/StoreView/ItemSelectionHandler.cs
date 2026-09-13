using Assets.Source.Scripts.Items;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.Storage;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Pool;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using YG;

namespace Assets.Source.Scripts.UI.StoreView
{
    public class ItemSelectionHandler : MonoBehaviour
    {
        [SerializeField] private Inventory _inventory;
        [SerializeField] private Store _store;
        [SerializeField] private ItemViewPool _itemViewPool;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _equipButton;
        [SerializeField] private Button _selectedButton;
        [SerializeField] private TMP_Text _priceText;

        private Item _selectedItem;
        private NewItemView _currentItemView;

        private void Awake()
        {
            ValidateInitializeArguments();

            _buyButton.gameObject.SetActive(false);
            _selectedButton.gameObject.SetActive(false);
            _equipButton.gameObject.SetActive(true);

            _selectedButton.interactable = false;
        }

        private void Start()
        {
            _itemViewPool.ActivateAll();
            RefreshAllItemViews();
        }

        private void OnEnable()
        {
            _buyButton.onClick.AddListener(OnBuyButtonClicked);
            _equipButton.onClick.AddListener(OnEquipButtonClicked);
            _inventory.ItemBuyed += OnShowEquipButton;

            _currentItemView = null;
            _selectedItem = null;

            RefreshAllItemViews();
        }

        private void OnDisable()
        {
            _buyButton.onClick.RemoveListener(OnBuyButtonClicked);
            _equipButton.onClick.RemoveListener(OnEquipButtonClicked);
            _inventory.ItemBuyed -= OnShowEquipButton;
        }

        public void OnSelectShowedItem(NewItemView newItemView)
        {
            if (_currentItemView != null && _currentItemView != newItemView)
            {
                Item previousItem = _currentItemView.GetComponent<Item>();
                bool wasOwned = previousItem != null && _inventory.HasItem(previousItem.ID);
                bool wasEquipped = previousItem != null && _inventory.EquippedItem != null
                                   && previousItem.ID == _inventory.EquippedItem.ID;
                _currentItemView.UpdateVisualState(wasOwned, wasEquipped);
            }

            _currentItemView = newItemView;
            _selectedItem = _currentItemView.GetComponent<Item>();

            UpdateButtonsState();
            _currentItemView.Selected();
        }

        private void OnBuyButtonClicked()
        {
            if (_selectedItem == null)
                return;

            _store.BuyItem(_selectedItem);
            RefreshAllItemViews();
            UpdateButtonsState();
        }

        private void OnEquipButtonClicked()
        {
            if (_selectedItem == null)
                return;

            if (_inventory.HasItem(_selectedItem.ID) == false) 
                return;

            if (_selectedItem == _inventory.EquippedItem)
                return;

            _store.EquipItem(_selectedItem);
            RefreshAllItemViews();
            OnSelectShowedItem(_currentItemView);
        }

        private void OnShowEquipButton(NewItemView boughtItemView)
        {
            RefreshAllItemViews();

            if (_currentItemView == boughtItemView)
                UpdateButtonsState();
        }

        private void UpdateButtonsState()
        {
            if (_selectedItem == null)
                return;

            if (_inventory.HasItem(_selectedItem.ID))
            {
                _buyButton.gameObject.SetActive(false);

                bool isSelected = _selectedItem != null &&
                                  _inventory.EquippedItem != null &&
                                  _selectedItem.ID == _inventory.EquippedItem.ID;

                _equipButton.gameObject.SetActive(isSelected == false);
                _selectedButton.gameObject.SetActive(isSelected);
            }
            else
            {
                _buyButton.gameObject.SetActive(true);
                _equipButton.gameObject.SetActive(false);
                _selectedButton.gameObject.SetActive(false);

                _priceText.text = _selectedItem.Price.ToString();
            }
        }
       
        private void RefreshAllItemViews()
        {
            string equippedItemID = YG2.saves.EquippedItemID;

            foreach (NewItemView view in _itemViewPool.Objects)
            {
                Item item = view.GetComponent<Item>();
                if (item == null) continue;

                bool isOwned = _inventory.HasItem(item.ID);
                bool isEquipped = !string.IsNullOrEmpty(equippedItemID) && item.ID == equippedItemID;
                view.UpdateVisualState(isOwned, isEquipped);
            }

            // Если текущий выбранный элемент не установлен и есть экипированный предмет,
            // находим соответствующий view и выделяем его
            if (_currentItemView == null && !string.IsNullOrEmpty(equippedItemID))
            {
                NewItemView equippedView = _itemViewPool.Objects.FirstOrDefault(view =>
                {
                    Item item = view.GetComponent<Item>();
                    return item != null && item.ID == equippedItemID;
                });

                if (equippedView != null)
                {
                    OnSelectShowedItem(equippedView);
                }
            }
            else if (_currentItemView != null)
            {
                _currentItemView.Selected();
            }

            //foreach (NewItemView view in _itemViewPool.Objects)
            //{
            //    Item item = view.GetComponent<Item>();
            //    if (item == null) continue;

            //    bool isOwned = _inventory.HasItem(item.ID);
            //    bool isEquipped = _inventory.EquippedItem != null && item.ID == _inventory.EquippedItem.ID;
            //    view.UpdateVisualState(isOwned, isEquipped);
            //}

            //if (_currentItemView != null)
            //{
            //    _currentItemView.Selected();
            //}
        }

        private void ValidateInitializeArguments()
        {
            Guard.NotNull(_inventory, nameof(_inventory));
            Guard.NotNull(_store, nameof(_store));
            Guard.NotNull(_buyButton, nameof(_buyButton));
            Guard.NotNull(_equipButton, nameof(_equipButton));
            Guard.NotNull(_priceText, nameof(_priceText));
        }
    }
}