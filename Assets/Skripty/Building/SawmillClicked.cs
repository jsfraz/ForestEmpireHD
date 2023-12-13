using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawmillClicked : MonoBehaviour
{
    public GameObject buildCanvas;

    public void Clicked()
    {
        buildCanvas.GetComponent<Build>().buildingHouse = false;
        buildCanvas.GetComponent<Build>().buildingForge = false;
        buildCanvas.GetComponent<Build>().buildingSawmill = true;
        buildCanvas.GetComponent<Build>().buildingField = false;
    }
}
