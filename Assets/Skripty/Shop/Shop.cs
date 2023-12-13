using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public Canvas shopCanvas;
    public GameObject box;
    public GameObject pfp;
    public Text shopkeeperText;
    public Text shopkeeperName;

    public GameObject village;
    public GameObject player;

    public TextAsset shopkeeperTextFile;
    private string[] shopkeeperTxt;
    public TextAsset fullInventory;
    public TextAsset lowMoney;
    public TextAsset enoughMoney;

    private bool writing = false;

    private void Start()
    {
        //box.SetActive(false);
        shopkeeperTxt = shopkeeperTextFile.text.Split("\n"[0]);
        shopkeeperName.text = shopkeeperTxt[0];
    }

    public void Hide()
    {
        StartCoroutine(HideShop());
        player.GetComponent<PlayerController>().enabled = true;
    }

    public void Show()
    {
        StartCoroutine(ShowShop());
        GameObject.Find("InventoryCanvas").GetComponent<Canvas>().enabled = false;
        player.GetComponent<PlayerController>().enabled = false;
    }

    private IEnumerator HideShop()
    {
        for (byte i = 0; i < 250; i += 25)
        {
            shopCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }
        shopCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 255);

        box.SetActive(false);

        for (byte i = 250; i > 0; i -= 25)
        {
            shopCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }
        shopCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

        shopCanvas.enabled = false;
        GameObject.Find("villageChunk30").GetComponent<ShopE>().shown = false;
    }

    private IEnumerator ShowShop()
    {
        shopCanvas.enabled = true;

        for (byte i = 0; i < 250; i += 25)
        {
            shopCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }

        box.SetActive(true);
        StartCoroutine(AnimateTextSound(shopkeeperTxt[1], pfp));

        for (byte i = 250; i > 0; i -= 25)
        {
            shopCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }
        shopCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
    }

    /*
    IEnumerator AnimateText(Text messageObject, string text)        //animování textu delay
    {
        int i = 0;
        messageObject.text = "";

        while (i < text.Length)
        {
            messageObject.text += text[i++];
            yield return null;
        }
    }
    */

    public void BuyItem(string type)
    {
        if (writing == false)
        {
            int money = 0;
            if (type == "apple")
                money = 1;
            if (type == "bread")
                money = 3;
            if (type == "meat")
                money = 5;
            if (type == "cake")
                money = 7;

            bool empty = false;
            int position = 0;
            for (int i = 0; i < village.GetComponent<ManageVillage>().player.inventory.Length; i++)
            {
                if (village.GetComponent<ManageVillage>().player.inventory[i] == "")
                {
                    empty = true;
                    position = i;
                    break;
                }
            }

            Debug.Log("money: " + village.GetComponent<ManageVillage>().player.money);
            if (village.GetComponent<ManageVillage>().player.money >= money)
            {
                if (empty)
                {
                    Debug.Log("buying " + type + " for " + money);
                    Debug.Log("position " + position + " is empty");

                    village.GetComponent<ManageVillage>().player.money -= money;
                    village.GetComponent<ManageVillage>().player.inventory[position] = type;
                    StartCoroutine(village.GetComponent<ManageVillage>().Save());
                    StartCoroutine(village.GetComponent<ManageVillage>().LoadGameInfo());

                    Debug.Log("money: " + village.GetComponent<ManageVillage>().player.money);

                    PlaySound(box.GetComponent<AudioSource>(), "Zvuky/sfx/buy", 0.5F, false);
                    StartCoroutine(AnimateTextSound(enoughMoney.text, pfp));
                }
                else
                    StartCoroutine(AnimateTextSound(fullInventory.text, pfp));
            }
            else
                StartCoroutine(AnimateTextSound(lowMoney.text, pfp));
        }
    }

    IEnumerator AnimateTextSound(string strComplete, GameObject audio)
    {
        writing = true;

        int i = 0;
        shopkeeperText.text = "";

        while (i < strComplete.Length)
        {
            char letter = strComplete[i++];
            shopkeeperText.text += letter;

            if (letter != ' ')
                PlaySound(audio.GetComponent<AudioSource>(), "Zvuky/sfx/Dialog/shopkeeper/" + Random.Range(1, 4), 0.2F, false);

            yield return null;
        }

        writing = false;
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