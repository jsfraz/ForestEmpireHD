using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildButtonClicked : MonoBehaviour
{
    public GameObject buildCanvas;

    //nastavení boolu
    public void Clicked()
    {
        buildCanvas.GetComponent<Build>().buildClicked = true;
    }
}
