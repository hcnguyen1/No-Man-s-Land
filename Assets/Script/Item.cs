using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] public string itemName;
    [SerializeField] public int quantity;
    [SerializeField] public Sprite sprite;

    [TextArea][SerializeField] public string itemDescription;
    private InventoryManager inventoryManager;
    public float pickupDelay = 2f;
    private bool canBePickedUp = false;

    void Start()
    {
        inventoryManager = GameObject.Find("UI").GetComponent<InventoryManager>();
        StartCoroutine(EnablePickupAfterDelay()); // Start delay on spawn
    }

    IEnumerator EnablePickupAfterDelay()
    {
        yield return new WaitForSeconds(pickupDelay);
        canBePickedUp = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBePickedUp)
            return; // Ignore during drop delay

        if (collision.gameObject.CompareTag("Player"))
        {
            int leftOverItems = inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);

            if (leftOverItems <= 0)
                Destroy(gameObject);
            else
            {
                quantity = leftOverItems;
                Destroy(gameObject);
            }
        }
    }
}
