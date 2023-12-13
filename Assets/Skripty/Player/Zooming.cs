using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zooming : MonoBehaviour
{
    public Camera cmr;       //kamera
    public float speed = 0.2f;      //rychlost zoomu (čím menší hodnota tím rychlejší)
    public float zoom_to;       //cílová hodnota

    void Start()
    {
        StartCoroutine(Zoom());     //udělá zoom/unzoom
    }

    IEnumerator Zoom()
    {
        for (float i = GetComponent<Camera>().GetComponent<Camera>().orthographicSize; i < zoom_to; i += 0.05f)
        {
            yield return new WaitForSeconds(speed);
            GetComponent<Camera>().GetComponent<Camera>().orthographicSize = i;
        }
    }
}
