using System.Collections.Generic;
using UnityEngine;
using System;
using Inventory.Model;

namespace Inventory.UI
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField]
        private ShopItemUI shopItemPrefab;

        [SerializeField]
        private RectTransform shopContentPanel; // Left side - shop items

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
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
