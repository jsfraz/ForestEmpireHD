using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BattleInventory : MonoBehaviour
{
    public bool lastFight = false;

    public Sprite apple;
    public Sprite bread;
    public Sprite meat;
    public Sprite cake;

    public TextAsset eatApple;
    public TextAsset eatBread;
    public TextAsset eatMeat;
    public TextAsset eatCake;

    private string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ForestEmpireHD";

    public void StartInventory(string[] inventory)
    {
        //zrušení průhlednosti
        for (int i = 0; i < inventory.Length; i++)
            GameObject.Find("ImageInventory" + i).GetComponent<Image>().color = new Color32(255, 255, 255, 255);

        for (int i = 0; i < inventory.Length; i++)
        {
            //přiřazení spritů
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

    public void ItemClicked()
    {
        int number = int.Parse(name.Replace("ImageInventory", ""));
        string sprite = GameObject.Find("ImageInventory" + number).GetComponent<Image>().sprite.name;
        int hp = 0;
        string message = "";
        if (sprite == "apple")
        {
            hp = 5;
            message = eatApple.text;
        }
        if (sprite == "bread")
        {
            hp = 10;
            message = eatBread.text;
        } 
        if (sprite == "meat")
        {
            hp = 15;
            message = eatMeat.text;
        }   
        if (sprite == "cake")
        {
            hp = 20;
            message = eatCake.text;
        }

        Debug.Log(lastFight);

        //fight nebo lastfight
        if (lastFight)
        {
            LastFight lastFightScript = GameObject.Find("Main Camera").GetComponent<LastFight>();
            lastFightScript.player.inventory[number] = "";

            PlaySound(GetComponent<AudioSource>(), "Zvuky/sfx/eat", 0.5F, false);
            int beforeHp = lastFightScript.player.hp;
            lastFightScript.player.hp += hp;
            if (lastFightScript.player.hp > lastFightScript.player.maxhp)
                lastFightScript.player.hp = lastFightScript.player.maxhp;

            StartCoroutine(lastFightScript.HpCounter(beforeHp, lastFightScript.player.hp, lastFightScript.player.maxhp, lastFightScript.playerHealthBar));

            GameObject.Find("InventoryCanvas").GetComponent<Canvas>().enabled = false;

            StartCoroutine(lastFightScript.MessageFadeIn(0.2F));
            StartCoroutine(lastFightScript.AnimateText(0.3F, lastFightScript.message, message));
            StartCoroutine(lastFightScript.EnemyActionDelay(4F));
        }
        
        if (lastFight == false)
        {
            Fight fightScript = GameObject.Find("Main Camera").GetComponent<Fight>();
            fightScript.player.inventory[number] = "";

            PlaySound(GetComponent<AudioSource>(), "Zvuky/sfx/eat", 0.5F, false);
            int beforeHp = fightScript.player.hp;
            fightScript.player.hp += hp;
            if (fightScript.player.hp > fightScript.player.maxhp)
                fightScript.player.hp = fightScript.player.maxhp;

            StartCoroutine(fightScript.HpCounter(beforeHp, fightScript.player.hp, fightScript.player.maxhp, fightScript.playerHealthBar));

            GameObject.Find("InventoryCanvas").GetComponent<Canvas>().enabled = false;

            StartCoroutine(fightScript.MessageFadeIn(0.2F));
            StartCoroutine(fightScript.AnimateText(0.3F, fightScript.message, message));
            StartCoroutine(fightScript.EnemyActionDelay(4F));
        }
    }

    public void CloseClicked()
    {
        GameObject.Find("InventoryCanvas").GetComponent<Canvas>().enabled = false;
        Fight fightScript = GameObject.Find("Main Camera").GetComponent<Fight>();
        StartCoroutine(fightScript.ButtonsFadeIn());
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

        //přehrání
        audio.Play();
    }
}