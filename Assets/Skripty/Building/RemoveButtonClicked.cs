using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveButtonClicked : MonoBehaviour
{
    public GameObject buildCanvas;

    //nastavení boolu
    public void Clicked()
    {
        buildCanvas.GetComponent<Build>().removeClicked = true;
    }
}
