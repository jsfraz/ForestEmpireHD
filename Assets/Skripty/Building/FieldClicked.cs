using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldClicked : MonoBehaviour
{
    public GameObject buildCanvas;

    public void Clicked()
    {
        buildCanvas.GetComponent<Build>().buildingHouse = false;
        buildCanvas.GetComponent<Build>().buildingForge = false;
        buildCanvas.GetComponent<Build>().buildingSawmill = false;
        buildCanvas.GetComponent<Build>().buildingField = true;
    }
}
