using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeHiding : MonoBehaviour
{
    public GameObject kmen;
    public GameObject jehlici;

    private Canvas mapCanvas;
    private bool animating = true;

    private void Start()
    {
        kmen.SetActive(false);
        jehlici.SetActive(false);
        mapCanvas = GameObject.Find("CanvasMap").GetComponent<Canvas>();
    }

    private void Update()
    {
        if (mapCanvas.enabled && animating)
        {
            animating = false;
            jehlici.GetComponent<Animator>().enabled = false;
        } 
        if (mapCanvas.enabled == false && animating == false)
        {
            animating = true;
            jehlici.GetComponent<Animator>().enabled = true;
        }
    }

    private void OnBecameVisible()
    {
        kmen.SetActive(true);
        jehlici.SetActive(true);
    }

    private void OnBecameInvisible()
    {
        kmen.SetActive(false);
        jehlici.SetActive(false);
    }
}
