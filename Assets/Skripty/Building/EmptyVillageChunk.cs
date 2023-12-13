using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyVillageChunk : MonoBehaviour
{
    public Sprite spriteRed;
    public Sprite spriteNormal;
    public Sprite spriteGreen;

    public GameObject villageHouse;
    public GameObject villageForge;
    public GameObject villageSawmill;
    public GameObject villageField;

    private Build buildScript;

    //start
    void Start()
    {
        buildScript = GameObject.Find("CanvasBuild").GetComponent<Build>();
    }

    private void Update()
    {
        //fix aby nezůstavalo políčko
        if (buildScript.canvasShown == false)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    //přebarvení
    private void OnMouseEnter()
    {
        //stavění
        if (GameObject.Find("CanvasBuild").GetComponent<Build>().building)
            GetComponent<SpriteRenderer>().sprite = spriteGreen;

        //boření
        if (GameObject.Find("CanvasBuild").GetComponent<Build>().removing)
            GetComponent<SpriteRenderer>().sprite = spriteRed;
    }

    //nastavení na normální barvu
    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().sprite = spriteNormal;
    }

    private void OnMouseDown()
    {
        if (buildScript.buildingHouse)
            GameObject.Find("Village").GetComponent<ManageVillage>().BuildOnEmpty(this.name, villageHouse, GetComponent<Transform>().position);

        if (buildScript.buildingForge)
            GameObject.Find("Village").GetComponent<ManageVillage>().BuildOnEmpty(this.name, villageForge, GetComponent<Transform>().position);

        if (buildScript.buildingSawmill)
            GameObject.Find("Village").GetComponent<ManageVillage>().BuildOnEmpty(this.name, villageSawmill, GetComponent<Transform>().position);

        if (buildScript.buildingField)
            GameObject.Find("Village").GetComponent<ManageVillage>().BuildOnEmpty(this.name, villageField, GetComponent<Transform>().position);
    }
}
