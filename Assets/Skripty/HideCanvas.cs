using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCanvas : MonoBehaviour
{
    private Canvas cnv;

    // Start is called before the first frame update
    void Start()
    {
        cnv = GetComponent<Canvas>();
        cnv.enabled = false;
        Debug.Log("Canvas disabled.");
    }
}
