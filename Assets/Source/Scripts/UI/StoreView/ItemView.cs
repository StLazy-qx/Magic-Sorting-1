using Assets.Source.Scripts.Items;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.UI.StoreView
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _imageScin;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private Button _equipButton;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Color _nonClickedColor;
        [SerializeField] private Color _clickedColor;
        [SerializeField] private Image _equipImage;

        private void Awake()
        {
            ValidateInitializeArguments();
        }

        public void Initialize(ItemSO itemData)
        {
            ValidateItemData(itemData);

            _imageScin.sprite = itemData.Icon;
            _priceText.text = itemData.Price.ToString();
        }

        public void HideBuyButton()
        {
            _buyButton.gameObject.SetActive(false);
        }

        public void SetSelectedState()
        {
            _equipButton.interactable = false;
            _backgroundImage.color = _nonClickedColor;
        }

        public void SetUnselectedState()
        {
            _equipButton.interactable = true;
            _backgroundImage.color = _clickedColor;
        }

        private void ValidateInitializeArguments()
        {
            if (_backgroundImage == null)
            {
                throw new NullReferenceException(
                    "Background image reference is missing in ItemView.");
            }

            if (_imageScin == null)
            {
                throw new NullReferenceException(
                    "Skin image reference is missing in ItemView.");
            }

            if (_priceText == null)
            {
                throw new NullReferenceException(
                    "Price text reference is missing in ItemView.");
            }
        }

        private void ValidateItemData(ItemSO itemData)
        {
            if (itemData == null)
            {
                throw new ArgumentNullException(nameof(itemData),
                    "ItemSO data cannot be null when initializing ItemView.");
            }

            if (itemData.Icon == null)
            {
                throw new ArgumentException(
                    "ItemSO Icon is missing.", nameof(itemData));
            }

            if (itemData.Price < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(itemData.Price),
                    "Item price cannot be negative.");
            }
        }
    }
}