using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Build : MonoBehaviour
{
    public Canvas buildCanvas;
    public Camera mainCamera;
    public Camera buildCamera;
    public GameObject player;
    public GameObject buildingsBar;
    private float fieldOfView;

    /*
    public Text textPopulation;
    public Text textMaterial;
    public Text textHouses;
    public Text textForges;
    public Text textSawmills;
    public Text textFields;
    public Text textMoney;
    */
    public Text textHP;
    public Text textAttack;
    public Text textDefense;
    public Text textLevel;

    public bool canvasEnabled = false;
    public bool canvasShown = false;

    private RectTransform buildingsBarTransform;
    private bool buildingBar = false;

    public bool buildClicked = false;
    public bool building = false;
    public bool buildingHouse = false;
    public bool buildingForge = false;
    public bool buildingSawmill = false;
    public bool buildingField = false;

    public bool removeClicked = false;
    public bool removing = false;

    private void Start()
    {
        //přesuntí buildingsBaru mimo kameru
        buildingsBarTransform = buildingsBar.GetComponent<RectTransform>();
        buildingsBarTransform.anchoredPosition = new Vector3(65, buildingsBarTransform.anchoredPosition.y);

        fieldOfView = buildCamera.fieldOfView;
    }

    void Update()
    {
        //esx zavření
        if (canvasEnabled && canvasShown && Input.GetKeyDown(KeyCode.Escape))
        {
            canvasEnabled = false;
            canvasShown = false;
            StartCoroutine(Hide());
        }

        //zobrazení
        if (canvasEnabled && canvasShown == false)
        {
            canvasShown = true;
            buildCamera.GetComponent<Transform>().position = new Vector3(179, -30, buildCamera.GetComponent<Transform>().position.z);
            fieldOfView = 17;
            buildCamera.fieldOfView = fieldOfView;
            GameObject.Find("InventoryCanvas").GetComponent<Canvas>().enabled = false;
            StartCoroutine(Show());
        }
        //skrytí
        if (canvasEnabled == false && canvasShown)
        {
            canvasShown = false;
            StartCoroutine(Hide());
        }

        //pokud je povolen canvas
        if (canvasShown && GameObject.Find("CanvasPause").GetComponent<Canvas>().enabled == false)
        {
            //kliknutí build
            if (buildClicked)
            {
                buildClicked = false;

                building = true;
                removing = false;

                //zobrazení a skrytí buildings baru
                if (buildingBar == false)
                {
                    buildingBar = true;
                    StartCoroutine(ShowBuildingsBar());
                }
                else
                {
                    buildingBar = false;
                    StartCoroutine(HideBuildingsBar());
                }
            }

            //kliknutí remove
            if (removeClicked)
            {
                removeClicked = false;

                building = false;
                removing = true;

                //skrytí buildings baru
                if (buildingBar)
                {
                    buildingBar = false;
                    StartCoroutine(HideBuildingsBar());
                }

                buildingHouse = false;
                buildingForge = false;
                buildingSawmill = false;
                buildingField = false;
            }

            //zoom
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                Debug.Log("zoom");
                if (fieldOfView >= 5)
                    fieldOfView -= 1;
                buildCamera.fieldOfView = fieldOfView;
            }
            //unzoom
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                Debug.Log("unzoom");
                if (fieldOfView <= 16)
                    fieldOfView += 1;
                buildCamera.fieldOfView = fieldOfView;

                //hranice
                float increment = (17 - fieldOfView) * 4;
                if (buildCamera.GetComponent<Transform>().position.x < 179 - increment)
                    buildCamera.GetComponent<Transform>().position = new Vector3(179 - increment, buildCamera.GetComponent<Transform>().position.y, buildCamera.GetComponent<Transform>().position.z);
                if (buildCamera.GetComponent<Transform>().position.x > 179 + increment)
                    buildCamera.GetComponent<Transform>().position = new Vector3(179 + increment, buildCamera.GetComponent<Transform>().position.y, buildCamera.GetComponent<Transform>().position.z);
                if (buildCamera.GetComponent<Transform>().position.y < -30 - increment)
                    buildCamera.GetComponent<Transform>().position = new Vector3(buildCamera.GetComponent<Transform>().position.x, -30 - increment, buildCamera.GetComponent<Transform>().position.z);
                if (buildCamera.GetComponent<Transform>().position.y > -30 + increment)
                    buildCamera.GetComponent<Transform>().position = new Vector3(buildCamera.GetComponent<Transform>().position.x, -30 + increment, buildCamera.GetComponent<Transform>().position.z);
            }
            //pohyb myši
            if (Input.GetMouseButton(1))
            {
                //pohyb myší
                float speed = 50 * Time.deltaTime;
                buildCamera.GetComponent<Transform>().position -= new Vector3(Input.GetAxis("Mouse X") * speed, Input.GetAxis("Mouse Y") * speed, 0);

                //hranice
                float increment = (17 - fieldOfView) * 4;
                if (buildCamera.GetComponent<Transform>().position.x < 179 - increment)
                    buildCamera.GetComponent<Transform>().position = new Vector3(179 - increment, buildCamera.GetComponent<Transform>().position.y, buildCamera.GetComponent<Transform>().position.z);
                if (buildCamera.GetComponent<Transform>().position.x > 179 + increment)
                    buildCamera.GetComponent<Transform>().position = new Vector3(179 + increment, buildCamera.GetComponent<Transform>().position.y, buildCamera.GetComponent<Transform>().position.z);
                if (buildCamera.GetComponent<Transform>().position.y < -30 - increment)
                    buildCamera.GetComponent<Transform>().position = new Vector3(buildCamera.GetComponent<Transform>().position.x, -30 - increment, buildCamera.GetComponent<Transform>().position.z);
                if (buildCamera.GetComponent<Transform>().position.y > -30 + increment)
                    buildCamera.GetComponent<Transform>().position = new Vector3(buildCamera.GetComponent<Transform>().position.x, -30 + increment, buildCamera.GetComponent<Transform>().position.z);
            }
        }
    }

    //zobrazí build canvas
    private IEnumerator Show()
    {
        for (byte i = 0; i < 250; i += 25)
        {
            buildCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }
        buildCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
        
        /*
        textPopulation.enabled = false;
        textHouses.enabled = false;
        textForges.enabled = false;
        textSawmills.enabled = false;
        textFields.enabled = false;
        */
        textHP.enabled = false;
        textAttack.enabled = false;
        textDefense.enabled = false;
        textLevel.enabled = false;
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Animator>().Play("IdleFront");
        //player.GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Find("InteractLetter").GetComponent<SpriteRenderer>().enabled = false;
        EnableCollision("village_empty");
        EnableSprite("village_empty");
        EnableCollision("field");
        buildCamera.enabled = true;
        mainCamera.enabled = false;
        buildCanvas.enabled = true;

        for (byte i = 250; i > 0; i -= 25)
        {
            buildCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }
        buildCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
    }

    //skryje build canvas
    private IEnumerator Hide()
    {
        for (byte i = 0; i < 250; i += 25)
        {
            buildCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }
        buildCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 255);

        /*
        textPopulation.enabled = true;
        textHouses.enabled = true;
        textForges.enabled = true;
        textSawmills.enabled = true;
        textFields.enabled = true;
        */
        textHP.enabled = true;
        textAttack.enabled = true;
        textDefense.enabled = true;
        textLevel.enabled = true;
        player.GetComponent<PlayerController>().enabled = true;
        player.GetComponent<Animator>().Play("IdleFront");
        //player.GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("InteractLetter").GetComponent<SpriteRenderer>().enabled = true;
        DisableCollision("village_empty");
        DisableSprite("village_empty");
        DisableCollision("field");
        mainCamera.enabled = true;
        buildCamera.enabled = false;
        buildCanvas.enabled = false;
        building = false;
        removing = false;
        buildingHouse = false;
        buildingForge = false;
        buildingSawmill = false;
        buildingField = false;

        for (byte i = 250; i > 0; i -= 25)
        {
            buildCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }
        buildCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
    }

    //zobrazí building bar
    private IEnumerator ShowBuildingsBar()
    {
        for (float i = 65; i > -65; i -= 10)
        {
            yield return null;
            buildingsBarTransform.anchoredPosition = new Vector3(i, buildingsBarTransform.anchoredPosition.y);
        }
        buildingsBarTransform.anchoredPosition = new Vector3(-65, buildingsBarTransform.anchoredPosition.y);
    }

    //skryje building bar
    private IEnumerator HideBuildingsBar()
    {
        for (float i = -65F; i < 65; i += 10)
        {
            yield return null;
            buildingsBarTransform.anchoredPosition = new Vector3(i, buildingsBarTransform.anchoredPosition.y);
        }
        buildingsBarTransform.anchoredPosition = new Vector3(65, buildingsBarTransform.anchoredPosition.y);
    }

    private void EnableCollision(string spriteName)
    {
        for (int i = 0; i < 100; i++)
        {
            SpriteRenderer chunkSpriteRenderer = GameObject.Find("villageChunk" + i).GetComponent<SpriteRenderer>();
            BoxCollider2D chunkBoxCollider2d = GameObject.Find("villageChunk" + i).GetComponent<BoxCollider2D>();
            if (chunkSpriteRenderer.sprite.name == spriteName)
                chunkBoxCollider2d.enabled = true;
        }
    }

    private void EnableSprite(string spriteName)
    {
        for (int i = 0; i < 100; i++)
        {
            SpriteRenderer chunkSpriteRenderer = GameObject.Find("villageChunk" + i).GetComponent<SpriteRenderer>();
            if (chunkSpriteRenderer.sprite.name == spriteName)
                chunkSpriteRenderer.enabled = true;
        }
    }

    private void DisableCollision(string spriteName)
    {
        for (int i = 0; i < 100; i++)
        {
            SpriteRenderer chunkSpriteRenderer = GameObject.Find("villageChunk" + i).GetComponent<SpriteRenderer>();
            BoxCollider2D chunkBoxCollider2d = GameObject.Find("villageChunk" + i).GetComponent<BoxCollider2D>();
            if (chunkSpriteRenderer.sprite.name == spriteName)
                chunkBoxCollider2d.enabled = false;
        }
    }

    private void DisableSprite(string spriteName)
    {
        for (int i = 0; i < 100; i++)
        {
            SpriteRenderer chunkSpriteRenderer = GameObject.Find("villageChunk" + i).GetComponent<SpriteRenderer>();
            if (chunkSpriteRenderer.sprite.name == spriteName)
                chunkSpriteRenderer.enabled = false;
        }
    }
}