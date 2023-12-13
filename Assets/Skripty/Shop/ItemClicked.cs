using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemClicked : MonoBehaviour
{
    public Canvas shopCanvas;

    public void BuyItem()
    {
        string type = GetComponent<Image>().sprite.name;
        shopCanvas.GetComponent<Shop>().BuyItem(type);
    }
}
