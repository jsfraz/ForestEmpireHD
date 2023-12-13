using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseClicked : MonoBehaviour
{
    public GameObject buildCanvas;

    public void Clicked()
    {
        buildCanvas.GetComponent<Build>().buildingHouse = true;
        buildCanvas.GetComponent<Build>().buildingForge = false;
        buildCanvas.GetComponent<Build>().buildingSawmill = false;
        buildCanvas.GetComponent<Build>().buildingField = false;
    }
}
