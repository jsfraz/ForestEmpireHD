using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemHover : MonoBehaviour
{
    public Text descriptionText;

    public TextAsset apple;
    public TextAsset bread;
    public TextAsset meat;
    public TextAsset cake;
    public TextAsset defaultText;
    public TextAsset upgrade1;
    public TextAsset upgrade2;
    public TextAsset upgrade3;

    private void Start()
    {
        descriptionText.text = defaultText.text;
    }

    public void ShowDescription()
    {
        if (GetComponent<Image>().sprite != null)
        {
            string sprite = GetComponent<Image>().sprite.name;

            if (sprite == "apple")
                descriptionText.text = apple.text;
            if (sprite == "bread")
                descriptionText.text = bread.text;
            if (sprite == "meat")
                descriptionText.text = meat.text;
            if (sprite == "cake")
                descriptionText.text = cake.text;
            if (sprite == "upgrade1")
                descriptionText.text = upgrade1.text;
            if (sprite == "upgrade2")
                descriptionText.text = upgrade2.text;
            if (sprite == "upgrade3")
                descriptionText.text = upgrade3.text;
        }
    }

    public void ResetDescription()
    {
        descriptionText.text = defaultText.text;
    }
}
