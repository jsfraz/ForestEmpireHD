using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapSceneChange : MonoBehaviour
{
    public Color loadToColor = Color.black;     //barva
    public string sceneName;        //Scene
    private string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ForestEmpireHD";

    private void OnTriggerEnter2D(Collider2D collision)     //kolize s hr·Ëem
    {
        if (collision.name == "Hero")
        {
            GameObject player = GameObject.Find("Hero");

            //pozice hr·Ëe
            string[] playerPosition = File.ReadAllLines(saveFolder + "/position");
            playerPosition[0] = player.GetComponent<Transform>().position.x.ToString();     //X
            playerPosition[1] = (player.GetComponent<Transform>().position.y - 1).ToString();        //Y
            playerPosition[2] = player.GetComponent<Transform>().position.z.ToString();       //Z
            File.WriteAllLines(saveFolder + "/position", playerPosition);

            GameObject.Find("Village").GetComponent<ManageVillage>().SaveTime();

            Initiate.Fade(sceneName, loadToColor, 3f);      //Fade script
            Debug.Log("Changing scene to: " + sceneName + ".");
        }
    }
}
