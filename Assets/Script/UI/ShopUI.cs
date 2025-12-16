using System.Collections.Generic;
using UnityEngine;
using System;
using Inventory.Model;

namespace Inventory.UI
{
    // while the shopItem UI manipulates the visuals and purchases, this class works on the UI itself like whether the text appears 
    // The creation of the shop is called in InitializeShop and it pulls the array from the inspector in unityhub. 
    // whether the user has enough currency or not determines if the items affordability marker shows up and whether the item is dimmed or brightened. 
    public class ShopUI : MonoBehaviour
    {
        [SerializeField]
        private ShopItemUI shopItemPrefab;

        [SerializeField]
        private RectTransform shopContentPanel; // Left side - shop items

        [SerializeField]
        private GameObject shopPanel; // The panel to show/hide

        [SerializeField]
        private GameObject inventoryText; // "Inventory" label

        [SerializeField]
        private GameObject shopText; // "Shop" label

        private List<ShopItemUI> shopItemSlots = new List<ShopItemUI>();

        public event Action<ItemSO, int> OnItemPurchaseRequested;

        private void Awake()
        {
            Hide();
        }

        public void InitializeShop(List<ItemSO> shopItems, int playerCurrency)
        {
            // Clear existing slots
            ClearShop();

            // Create slots for each shop item
            foreach (ItemSO item in shopItems)
            {
                // Skip null items
                if (item == null) continue;
                
                ShopItemUI slot = Instantiate(shopItemPrefab, shopContentPanel);
                slot.transform.SetParent(shopContentPanel);
                
                bool canAfford = playerCurrency >= item.Price;
                slot.SetData(item, item.Price, canAfford);
                slot.OnBuyClicked += HandlePurchaseRequest;
                
                shopItemSlots.Add(slot);
            }
        }

        public void UpdateAffordability(int playerCurrency, List<ItemSO> shopItems)
        {
            for (int i = 0; i < shopItemSlots.Count && i < shopItems.Count; i++)
            {
                bool canAfford = playerCurrency >= shopItems[i].Price;
                shopItemSlots[i].SetData(shopItems[i], shopItems[i].Price, canAfford);
            }
        }

        private void ClearShop()
        {
            foreach (var slot in shopItemSlots)
            {
                if (slot != null)
                {
                    Destroy(slot.gameObject);
                }
            }
            shopItemSlots.Clear();
        }

        private void HandlePurchaseRequest(ItemSO item, int price)
        {
            OnItemPurchaseRequested?.Invoke(item, price);
        }

        public void Show()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(true);
            }
            else
            {
                gameObject.SetActive(true);
            }

            // Show text labels
            if (inventoryText != null) inventoryText.SetActive(true);
            if (shopText != null) shopText.SetActive(true);
        }

        public void Hide()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }

            // Hide text labels
            if (inventoryText != null) inventoryText.SetActive(false);
            if (shopText != null) shopText.SetActive(false);
        }
    }
}
