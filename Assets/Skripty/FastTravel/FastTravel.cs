using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FastTravel : MonoBehaviour
{
    public GameObject player;
    public ManageVillage village;
    public Dropdown dropdownMenu;
    public Canvas travelCanvas;
    public GameObject box;
    public Text textTravel;

    public TextAsset travelText;
    public TextAsset travelNoMoney;

    private string currentLocation;
    private bool shown = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Hide();
    }

    public void Travel()
    {
        string destination = "";
        if (dropdownMenu.value == 1)
            destination = "village";
        if (dropdownMenu.value == 2)
            destination = "cave";
        if (dropdownMenu.value == 3)
            destination = "house";
        if (dropdownMenu.value == 4)
            destination = "pond";

        //kontrola penìz
        if (village.player.money >= 5)
        {
            if (dropdownMenu.value != 0 && destination != currentLocation)
            {
                box.SetActive(false);
                shown = false;
                village.player.money -= 5;
                StartCoroutine(village.Save());
                StartCoroutine(village.LoadGameInfo());

                GameObject.Find("cart_" + currentLocation).GetComponent<Cart>().Travel(currentLocation, destination);
            }
        }
        else
            StartCoroutine(AnimateTextSound(travelNoMoney.text, textTravel.gameObject));
    }

    public void Hide()
    {
        if (shown)
        {
            shown = false;
            StartCoroutine(HideTable());
            player.GetComponent<PlayerController>().enabled = true;
        }
    }

    public void Show(string location)
    {
        if (shown == false)
        {
            shown = true;
            box.SetActive(true);
            currentLocation = location;
            StartCoroutine(ShowTable());
            player.GetComponent<PlayerController>().enabled = false;
            //fix aby se vypnula nimace
            player.GetComponent<Animator>().SetFloat("Horizontal", 0F);
            player.GetComponent<Animator>().SetFloat("Vertical", 0F);
        }
    }

    private IEnumerator HideTable()
    {
        for (byte i = 0; i < 250; i += 25)
        {
            travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }
        travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 255);

        box.SetActive(false);

        for (byte i = 250; i > 0; i -= 25)
        {
            travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }
        travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

        travelCanvas.enabled = false;
    }

    private IEnumerator ShowTable()
    {
        travelCanvas.enabled = true;

        for (byte i = 0; i < 250; i += 25)
        {
            travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }

        box.SetActive(true);
        StartCoroutine(AnimateTextSound(travelText.text, textTravel.gameObject));

        for (byte i = 250; i > 0; i -= 25)
        {
            travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }
        travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
    }

    IEnumerator AnimateTextSound(string strComplete, GameObject audio)
    {
        int i = 0;
        textTravel.text = "";

        while (i < strComplete.Length)
        {
            char letter = strComplete[i++];
            textTravel.text += letter;

            if (letter != ' ')
                PlaySound(audio.GetComponent<AudioSource>(), "Zvuky/sfx/letter" + Random.Range(1, 5), 0.2F, false);

            yield return null;
        }
    }

    private void PlaySound(AudioSource audio, string resource, float volume, bool loop)
    {
        audio.volume = volume;
        audio.clip = Resources.Load(resource) as AudioClip;     //inicializuje zvukový klip

        //nastavení loopování
        if (loop)
            audio.loop = true;
        else
            audio.loop = false;

        //pøehrání
        audio.Play();
    }
}
