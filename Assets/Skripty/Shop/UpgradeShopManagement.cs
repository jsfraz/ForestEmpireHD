using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShopManagement : MonoBehaviour
{
    public ManageVillage village;
    public GameObject box;

    // Start is called before the first frame update
    void Start()
    {
        Disable();
    }

    //z nìjakého dùvodu nefunguje v Update()
    private void Disable()
    {
        bool show = false;

        if (gameObject.name == "ImageUpgrade1" && village.player.level == 1)
            show = true;
        if (gameObject.name == "ImageUpgrade2" && village.player.level == 2)
            show = true;
        if (gameObject.name == "ImageUpgrade3" && village.player.level == 3)
            show = true;

        if (show == false)
            DisableButton(gameObject.name);
    }

    public void BuyUpgrade()
    {
        if (gameObject.name == "ImageUpgrade1" && village.player.money >= 300)
        {
            PlaySound(box.GetComponent<AudioSource>(), "Zvuky/sfx/buy", 0.5F, false);

            village.player.level = 2;
            village.player.money -= 300;
            village.player.maxhp = 21;
            village.player.hp += 1;
            village.player.attack = 11;

            DisableButton(gameObject.name);
            EnableButton("ImageUpgrade2");
        }
        if (gameObject.name == "ImageUpgrade2" && village.player.money >= 450)
        {
            PlaySound(box.GetComponent<AudioSource>(), "Zvuky/sfx/buy", 0.5F, false);
            
            village.player.level = 3;
            village.player.money -= 450;
            village.player.maxhp = 23;
            village.player.hp += 2;
            village.player.attack = 13;

            DisableButton(gameObject.name);
            EnableButton("ImageUpgrade3");
        }
        if (gameObject.name == "ImageUpgrade3" && village.player.money >= 600)
        {
            PlaySound(box.GetComponent<AudioSource>(), "Zvuky/sfx/buy", 0.5F, false);

            village.player.level = 4;
            village.player.money -= 600;
            village.player.maxhp = 26;
            village.player.hp += 3;
            village.player.attack = 15;

            DisableButton(gameObject.name);
        }

        StartCoroutine(village.Save());
        StartCoroutine(village.LoadGameInfo());
    }

    private void DisableButton(string buttonName)
    {
        GameObject obj = GameObject.Find(buttonName);

        obj.GetComponent<Image>().color = new Color32(70, 70, 70, 255);
        obj.GetComponent<Button>().enabled = false;
    }

    private void EnableButton(string buttonName)
    {
        GameObject obj = GameObject.Find(buttonName);

        obj.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        obj.GetComponent<Button>().enabled = true;
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
