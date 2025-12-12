using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu(fileName = "ShopInventory", menuName = "Shop/Shop Inventory")]
    public class ShopInventorySO : ScriptableObject
    {
        [SerializeField]
        private List<ItemSO> shopItems = new List<ItemSO>(); // Items the merchant sells

        public List<ItemSO> GetShopItems()
        {
            return shopItems;
        }

        public int GetItemPrice(ItemSO item)
        {
            if (shopItems.Contains(item))
            {
                return item.Price;
            }
            return 0;
        }
    }
}
