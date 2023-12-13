using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatCaveOut : MonoBehaviour
{
    public Color loadToColor = Color.black;     //barva
    public string sceneName;        //Scene

    private void OnTriggerEnter2D(Collider2D collision)     //kolize s hr·Ëem
    {
        if (collision.name == "Hero")
        {
            Initiate.Fade(sceneName, loadToColor, 3f);      //Fade script
            Debug.Log("Changing scene to: " + sceneName + ".");
        }
    }
}
