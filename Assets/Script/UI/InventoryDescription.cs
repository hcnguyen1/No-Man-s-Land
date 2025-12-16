using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Inventory.UI
{

    // Each inventory item needs to make sure their image, title, and description are updated and not NULL hence SetDescription
    // This class adjusts those features and uses reset in case the item is no longer there, so that there would be no afterimage or remainders.
    // this is ResetDescription. 
    public class InventoryDescription : MonoBehaviour
    {

        [SerializeField]
        private Image itemImage;

        [SerializeField]
        private TMP_Text title;

        [SerializeField]
        private TMP_Text description;

        public void Awake()
        {
            ResetDescription();
        }


        public void ResetDescription()
        {
            this.itemImage.gameObject.SetActive(false);
            this.title.text = "";
            this.description.text = "";
        }

        public void SetDescription(Sprite sprite, string itemName, string itemDescription)
        {
            this.itemImage.gameObject.SetActive(true);
            this.itemImage.sprite = sprite;
            this.title.text = itemName;
            this.description.text = itemDescription;
        }

    }

}
