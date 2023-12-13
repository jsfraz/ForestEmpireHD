using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemClicked : MonoBehaviour
{
    public GameObject inventoryCanvas; 

    public void Clicked()
    {
        if (GetComponent<Image>().sprite != null)
        {
            string spriteName = GetComponent<Image>().sprite.name;
            int number = int.Parse(this.name.Replace("ImageInventory", ""));
            inventoryCanvas.GetComponent<Inventory>().UseItem(spriteName, number);
        }
    }
}
