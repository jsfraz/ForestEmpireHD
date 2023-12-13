using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseEditMode : MonoBehaviour
{
    public GameObject buildCanvas;

    public void Close()
    {
        buildCanvas.GetComponent<Build>().canvasEnabled = false;
    }
}
