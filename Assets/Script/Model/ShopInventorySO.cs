using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{

    // This gives the merchant shop an interactive feature that connects their shop to the players inventory.
    // We can get the shop items from the field in the form of an array as well as the price of each item. 
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
