using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.UI;

// Mousefollower creates a visual afterimage of the itemSlot image, and quantity so the player can visually feel the item being moved.
// then it will update based on the new position of the item, or whether it moved or not. 
public class MouseFollower : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private InventoryItem item;


    public void Awake()
    {
        if (canvas == null)
        {
            canvas = transform.root.GetComponent<Canvas>();
        }
        item = GetComponentInChildren<InventoryItem>();
    }

    public void SetData(Sprite sprite, int quantity)
    {
        item.SetData(sprite, quantity);
    }

    void Update() // this function basically allows us to take the mouse position, and transform the screen rect thats usually in the inspector, as well as move the camera
    {
        if (canvas == null) return; // Safety check
        
        Vector2 position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, Input.mousePosition, canvas.worldCamera, out position);
        transform.position = canvas.transform.TransformPoint(position); // the out is basically c#'s reference operator where it where to go during the time of the call/execution
    }

    public void Toggle(bool value)
    {
        gameObject.SetActive(value);
    }
}
