using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeClicked : MonoBehaviour
{
    public GameObject buildCanvas;

    public void Clicked()
    {
        buildCanvas.GetComponent<Build>().buildingHouse = false;
        buildCanvas.GetComponent<Build>().buildingForge = true;
        buildCanvas.GetComponent<Build>().buildingSawmill = false;
        buildCanvas.GetComponent<Build>().buildingField = false;
    }
}
