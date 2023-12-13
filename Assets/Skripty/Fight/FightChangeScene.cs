using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FightChangeScene : MonoBehaviour
{
    public Color loadToColor = Color.black;     //barva
    public string sceneName;        //Scene
    private string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ForestEmpireHD";

    private void OnTriggerEnter2D(Collider2D collision)     //kolize s hr·Ëem
    {
        if (collision.name == "Hero")
        {
            //pozice hr·Ëe
            string[] playerPosition = File.ReadAllLines(saveFolder + "/position");
            playerPosition[0] = "-98,5";     //X
            playerPosition[1] = "174";        //Y
            playerPosition[2] = "-145,4";       //Z
            File.WriteAllLines(saveFolder + "/position", playerPosition);

            GameObject.Find("Village").GetComponent<ManageVillage>().SaveTime();

            Initiate.Fade(sceneName, loadToColor, 3f);      //Fade script
            Debug.Log("Changing scene to: " + sceneName + ".");
        }
    }
}
