using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject village;
    public Canvas inventoryCanvas;
    public GameObject player;

    public Sprite apple;
    public Sprite bread;
    public Sprite meat;
    public Sprite cake;

    public TextAsset eatApple;
    public TextAsset eatBread;
    public TextAsset eatMeat;
    public TextAsset eatCake;

    void Update()
    {
        bool valid = false;
        if (GameObject.Find("CanvasBuild").GetComponent<Canvas>().enabled == false && GameObject.Find("CanvasShop").GetComponent<Canvas>().enabled == false && GameObject.Find("CanvasFastTravel").GetComponent<Canvas>().enabled == false && GameObject.Find("DialogCanvas").GetComponent<Canvas>().enabled == false && GameObject.Find("CanvasMap").GetComponent<Canvas>().enabled == false)
            valid = true;

        if (Input.GetKeyDown(KeyCode.I) && inventoryCanvas.enabled == false && valid)
        {
            player.GetComponent<PlayerController>().enabled = false;
            inventoryCanvas.enabled = true;

            StartInventory(village.GetComponent<ManageVillage>().player.inventory);
        }
    }

    public void StartInventory(string[] inventory)
    {
        PlaySound(inventoryCanvas.GetComponent<AudioSource>(), "Zvuky/sfx/inventory", 0.5F, false);

        //zrušení prùhlednosti
        for (int i = 0; i < inventory.Length; i++)
            GameObject.Find("ImageInventory" + i).GetComponent<Image>().color = new Color32(255, 255, 255, 255);

        for (int i = 0; i < inventory.Length; i++)
        {
            //pøiøazení spritù
            if (inventory[i] == "apple")
                GameObject.Find("ImageInventory" + i).GetComponent<Image>().sprite = apple;
            if (inventory[i] == "bread")
                GameObject.Find("ImageInventory" + i).GetComponent<Image>().sprite = bread;
            if (inventory[i] == "meat")
                GameObject.Find("ImageInventory" + i).GetComponent<Image>().sprite = meat;
            if (inventory[i] == "cake")
                GameObject.Find("ImageInventory" + i).GetComponent<Image>().sprite = cake;
            if (inventory[i] == "")
            {
                GameObject.Find("ImageInventory" + i).GetComponent<Image>().sprite = null;
                GameObject.Find("ImageInventory" + i).GetComponent<Image>().color = new Color32(255, 255, 255, 0);
            }
        }
    }

    public void Close()
    {
        inventoryCanvas.enabled = false;
        player.GetComponent<PlayerController>().enabled = true;
    }

    public void UseItem(string type, int number)
    {
        Debug.Log("using " + type + " on position ");

        int hp = 0;
        if (type == "apple")
            hp = 5;
        if (type == "bread")
            hp = 10;
        if (type == "meat")
            hp = 15;
        if (type == "cake")
            hp = 20;

        village.GetComponent<ManageVillage>().player.hp += hp;
        PlaySound(inventoryCanvas.GetComponent<AudioSource>(), "Zvuky/sfx/eat", 0.5F, false);

        if (village.GetComponent<ManageVillage>().player.hp > village.GetComponent<ManageVillage>().player.maxhp)
            village.GetComponent<ManageVillage>().player.hp = village.GetComponent<ManageVillage>().player.maxhp;

        village.GetComponent<ManageVillage>().player.inventory[number] = "";
        GameObject.Find("ImageInventory" + number).GetComponent<Image>().sprite = null;
        GameObject.Find("ImageInventory" + number).GetComponent<Image>().color = new Color32(255, 255, 255, 0);

        StartCoroutine(village.GetComponent<ManageVillage>().Save());
        StartCoroutine(village.GetComponent<ManageVillage>().LoadGameInfo());
    }

    private void PlaySound(AudioSource audio, string resource, float volume, bool loop)
    {
        audio.volume = volume;
        audio.clip = Resources.Load(resource) as AudioClip;     //inicializuje zvukový klip

        //nastavení loopování
        if (loop)
            audio.loop = true;
        else
            audio.loop = false;

        //pøehrání
        audio.Play();
    }
}
