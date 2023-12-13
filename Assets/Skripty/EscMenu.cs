using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscMenu : MonoBehaviour
{
    //Time.timescale 0 pozastraví hru

    public Dropdown dropdownResolution;
    public Dropdown dropdownScreenType;
    public Button menuButton;
    public Button saveButton;
    public Button exitButton;
    public bool saveOption;     //urèuje jestli je možné ukládat

    private bool shown = false;
    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Hero");

        if (saveOption == false)
            GameObject.Find("ButtonSave").GetComponent<Button>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //zobrazení a skrytí menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (shown)
                Hide();
            else
                Show();
        }
    }

    private void Show()
    {
        if (saveOption == false)
        {
            shown = true;
            dropdownResolution.enabled = true;
            dropdownScreenType.enabled = true;
            menuButton.enabled = true;
            saveButton.enabled = true;
            exitButton.enabled = true;
            GetComponent<Canvas>().enabled = true;
            Time.timeScale = 0;
        }
        else
        {
            if (GameObject.Find("CanvasBuild").GetComponent<Canvas>().enabled == false && GameObject.Find("CanvasFastTravel").GetComponent<Canvas>().enabled == false && GameObject.Find("CanvasMap").GetComponent<Canvas>().enabled == false && GameObject.Find("DialogCanvas").GetComponent<Canvas>().enabled == false && GameObject.Find("InventoryCanvas").GetComponent<Canvas>().enabled == false)
            {
                shown = true;
                dropdownResolution.enabled = true;
                dropdownScreenType.enabled = true;
                menuButton.enabled = true;
                saveButton.enabled = true;
                exitButton.enabled = true;
                GetComponent<Canvas>().enabled = true;
                player.GetComponent<PlayerController>().enabled = false;
                Time.timeScale = 0;
            }
        }
    }

    private void Hide()
    {
        shown = false;
        GetComponent<Canvas>().enabled = false;
        dropdownResolution.enabled = false;
        dropdownScreenType.enabled = false;
        menuButton.enabled = false;
        saveButton.enabled = false;
        exitButton.enabled = false;

        if (player != null)
            player.GetComponent<PlayerController>().enabled = true;

        Time.timeScale = 1;
    }

    public void MenuButton()
    {
        GetComponent<Canvas>().enabled = false;
        Time.timeScale = 1;

        SceneChanger changer = GetComponent<SceneChanger>();
        changer.sceneName = "Menu";

        //uložení
        if (saveOption)
            Save();

        changer.ChangeScene();
    }

    public void SaveButton()
    {
        Hide();
        Save();
    }

    void Save()
    {
        StartCoroutine(GameObject.Find("Village").GetComponent<ManageVillage>().Save());
        StartCoroutine(GameObject.Find("Village").GetComponent<ManageVillage>().LoadGameInfo());
    }

    public void ExitButton()
    {
        //uložení
        if (saveOption)
            Save();
        Application.Quit();
    }

    public void DropdownOptionChanged()
    {
        /*
         * Módy
        ExclusiveFullScreen	Exclusive Mode.
        FullScreenWindow	Fullscreen window.
        MaximizedWindow	Maximized window.
        Windowed	Windowed.
        */

        //rozlišení
        int resOption = dropdownResolution.value;
        int x = Screen.width;
        int y = Screen.height;
        if (resOption == 0)
        {
            x = 1280;
            y = 1024;
        }
        if (resOption == 1)
        {
            x = 1280;
            y = 720;
        }
        if (resOption == 2)
        {
            x = 1366;
            y = 768;
        }
        if (resOption == 3)
        {
            x = 1600;
            y = 900;
        }
        if (resOption == 4)
        {
            x = 1920;
            y = 1080;
        }

        //mód
        int typeOption = dropdownScreenType.value;
        FullScreenMode mode = Screen.fullScreenMode;
        if (typeOption == 0)
            mode = FullScreenMode.ExclusiveFullScreen;
        if (typeOption == 1)
            mode = FullScreenMode.FullScreenWindow;
        if (typeOption == 2)
            mode = FullScreenMode.Windowed;

        //nastavení rozlišení a módu
        Screen.SetResolution(x, y, mode);
    }
}
