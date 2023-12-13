using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FirstSave : MonoBehaviour
{
    public static void CreateSave()
    {
        //savefolder
        string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ForestEmpireHD";
        if (Directory.Exists(saveFolder) == false)
            Directory.CreateDirectory(saveFolder);

        //vesnice
        string[,] village = new string[10, 10];       //pole vesnice
        village[3, 0] = "shop";
        village[4, 0] = "workshop";
        village[4, 1] = "house";
        village[4, 2] = "house";
        village[4, 3] = "house";
        village[5, 0] = "field";
        village[5, 1] = "sawmill";
        village[5, 2] = "forge";

        string[] save = new string[100];
        int position = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                save[position] = village[i, j];
                position++;
            }
        }
        
        File.WriteAllLines(saveFolder + "/village_buildings", save);


        //statistiky hráče
        string[] stats = new string[8];
        stats[0] = "1";       //level
        stats[1] = "20";      //maxhp
        stats[2] = "1";       //hp
        stats[3] = "10";       //attack
        stats[4] = "5";       //defense
        stats[5] = "50";      //money
        stats[6] = "50";       //material
        stats[7] = "true";      //true = spuštění poprvé, false = spuštění není poprvé

        File.WriteAllLines(saveFolder + "/stats", stats);


        //inventář hráče
        string[] inventory = new string[4];
        stats[0] = "";
        stats[1] = "";
        stats[2] = "";
        stats[3] = "";

        File.WriteAllLines(saveFolder + "/inventory", inventory);

        //pozice na mapě
        string[] playerPosition = new string[3];
        playerPosition[0] = "-1,5";     //X
        playerPosition[1] = "-24,3";        //Y
        playerPosition[2] = "-0";       //Z

        File.WriteAllLines(saveFolder + "/position", playerPosition);


        //světlo
        string[] time = { "0", "0,1" };
        File.WriteAllLines(saveFolder + "/time", time);


        //lastfight
        string[] lastFight = { "false" };
        File.WriteAllLines(saveFolder + "/lastfight", lastFight);


        //eventy
        Directory.CreateDirectory(saveFolder + "/events");

        //hledání kočky
        string[] oldLady = { "false", "false" };
        File.WriteAllLines(saveFolder + "/events/oldlady", oldLady);
    }
}
