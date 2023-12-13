using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public Canvas mapCanvas;
    public GameObject player;

    private bool shown = false;
    private float fieldOfView;

    private void Start()
    {
        fieldOfView = GetComponent<Camera>().fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        //move kamery
        if (shown && GameObject.Find("CanvasPause").GetComponent<Canvas>().enabled == false)
            CameraBehaviour();

        //otevírání mapy
        if (Input.GetKeyDown(KeyCode.M) && GameObject.Find("CanvasBuild").GetComponent<Canvas>().enabled == false && GameObject.Find("CanvasShop").GetComponent<Canvas>().enabled == false && GameObject.Find("CanvasFastTravel").GetComponent<Canvas>().enabled == false && GameObject.Find("DialogCanvas").GetComponent<Canvas>().enabled == false && GameObject.Find("CanvasPause").GetComponent<Canvas>().enabled == false)
        {
            if (shown)
            {
                mapCanvas.enabled = false;
                GetComponent<Camera>().enabled = false;
                player.GetComponent<PlayerController>().enabled = true;
                shown = false;
            }
            else
            {
                player.GetComponent<Animator>().SetFloat("Horizontal", 0F);
                player.GetComponent<Animator>().SetFloat("Vertical", 0F);
                player.GetComponent<PlayerController>().enabled = false;
                GetComponent<Transform>().position = new Vector3(player.GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y, GetComponent<Transform>().position.z);
                fieldOfView = 6;
                GetComponent<Camera>().fieldOfView = 6;
                GetComponent<Camera>().enabled = true;
                mapCanvas.enabled = true;
                shown = true;
            }
        }
    }

    private void CameraBehaviour()
    {
        //zoom
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            Debug.Log("zoom");
            if (fieldOfView >= 8)
                fieldOfView -= 2F;
            GetComponent<Camera>().fieldOfView = fieldOfView;
        }
        //unzoom
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            Debug.Log("unzoom");
            if (fieldOfView <= 57)
                fieldOfView += 2F;
            GetComponent<Camera>().fieldOfView = fieldOfView;

            //hranice
            float increment = (59 - fieldOfView) * 8;
            if (GetComponent<Transform>().position.x < 122 - increment)
                GetComponent<Transform>().position = new Vector3(122 - increment, GetComponent<Transform>().position.y, GetComponent<Transform>().position.z);
            if (GetComponent<Transform>().position.x > 122 + increment)
                GetComponent<Transform>().position = new Vector3(122 + increment, GetComponent<Transform>().position.y, GetComponent<Transform>().position.z);
            if (GetComponent<Transform>().position.y < -20 - increment)
                GetComponent<Transform>().position = new Vector3(GetComponent<Transform>().position.x, -20 - increment, GetComponent<Transform>().position.z);
            if (GetComponent<Transform>().position.y > -20 + increment)
                GetComponent<Transform>().position = new Vector3(GetComponent<Transform>().position.x, -20 + increment, GetComponent<Transform>().position.z);
        }
        //pohyb myši
        if (Input.GetMouseButton(1))
        {
            //pohyb myší
            float speed = 50 * Time.deltaTime;
            GetComponent<Transform>().position -= new Vector3(Input.GetAxis("Mouse X") * speed, Input.GetAxis("Mouse Y") * speed, 0);

            //hranice
            float increment = (59 - fieldOfView) * 8;
            if (GetComponent<Transform>().position.x < 122 - increment)
                GetComponent<Transform>().position = new Vector3(122 - increment, GetComponent<Transform>().position.y, GetComponent<Transform>().position.z);
            if (GetComponent<Transform>().position.x > 122 + increment)
                GetComponent<Transform>().position = new Vector3(122 + increment, GetComponent<Transform>().position.y, GetComponent<Transform>().position.z);
            if (GetComponent<Transform>().position.y < -20 - increment)
                GetComponent<Transform>().position = new Vector3(GetComponent<Transform>().position.x, -20 - increment, GetComponent<Transform>().position.z);
            if (GetComponent<Transform>().position.y > -20 + increment)
                GetComponent<Transform>().position = new Vector3(GetComponent<Transform>().position.x, -20 + increment, GetComponent<Transform>().position.z);
        }
    }
}