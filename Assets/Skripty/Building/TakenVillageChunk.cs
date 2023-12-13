using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakenVillageChunk : MonoBehaviour
{
    public GameObject emptyChunk;

    //přebarvení
    private void OnMouseEnter()
    {
        //boření
        if (GameObject.Find("CanvasBuild").GetComponent<Build>().removing)
            GetComponent<SpriteRenderer>().color = new Color32(255, 180, 180, 255);
    }

    //boření
    private void OnMouseDown()
    {
        if (GameObject.Find("CanvasBuild").GetComponent<Build>().removing)
        {
            Debug.Log("removing " + name);

            GameObject.Find("Village").GetComponent<ManageVillage>().RemoveBuilding(name, GameObject.Find(name), emptyChunk, GetComponent<Transform>().position);
        }
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
    }
}
