using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Inventory.Model;

namespace Inventory.UI
{
    public class ShopItemUI : MonoBehaviour
    {
        [SerializeField]
        private Image itemImage;

        [SerializeField]
        private TMP_Text itemNameText;

        [SerializeField]
        private TMP_Text priceText;

        [SerializeField]
        private TMP_Text insufficientFundsText;

        [SerializeField]
        private Button buyButton;

        [SerializeField]
        private Color affordableColor = Color.white;

        [SerializeField]
        private Color unaffordableColor = Color.gray;

        [SerializeField]
        private Color affordablePriceColor = Color.white;

        [SerializeField]
        private Color unaffordablePriceColor = Color.red;

        private ItemSO currentItem;
        private int currentPrice;

        public event Action<ItemSO, int> OnBuyClicked;

        private void Awake()
        {
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(HandleBuyClick);
            }

            if (insufficientFundsText != null)
            {
                insufficientFundsText.gameObject.SetActive(false);
            }
        }

        public void SetData(ItemSO item, int price, bool canAfford)
        {
            currentItem = item;
            currentPrice = price;

            if (itemImage != null)
            {
                itemImage.sprite = item.ItemImage;
                itemImage.color = canAfford ? affordableColor : unaffordableColor;
            }

            if (itemNameText != null)
            {
                itemNameText.text = item.Name;
            }

            if (priceText != null)
            {
                priceText.text = $"{price}G";
                priceText.color = canAfford ? affordablePriceColor : unaffordablePriceColor;
            }

            if (insufficientFundsText != null)
            {
                insufficientFundsText.gameObject.SetActive(!canAfford);
            }

            if (buyButton != null)
            {
                buyButton.interactable = canAfford;
            }
        }

        public void ResetData()
        {
            if (itemImage != null)
            {
                itemImage.sprite = null;
                itemImage.color = affordableColor;
            }

            if (itemNameText != null)
            {
                itemNameText.text = "";
            }

            if (priceText != null)
            {
                priceText.text = "";
            }

            if (insufficientFundsText != null)
            {
                insufficientFundsText.gameObject.SetActive(false);
            }

            currentItem = null;
            currentPrice = 0;
        }

        private void HandleBuyClick()
        {
            if (currentItem != null)
            {
                OnBuyClicked?.Invoke(currentItem, currentPrice);
            }
        }
    }
}
