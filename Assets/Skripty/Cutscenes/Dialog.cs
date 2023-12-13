using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class Dialog : MonoBehaviour
{
    //objekty
    public Canvas canvas;     //DialogCanvas
    public Image pfp_object;       //Obrázek
    public Text dialog_name;        //jméno
    public Text dialog_text;        //text
    public Text dialog_position;        //pozice v dialogu
    public GameObject dialog_box;
    private Dialog dialog_script;       //Dialog skript

    //argumenty
    public Sprite profile_picture;      //obrázek postavy
    public Color name_color;      //barva jména
    public string text_file;     //soubor s dialogem

    //proměnné
    private string[] array;     //pole s řádky
    public int position = 1;       //pozice v dialogu (řádky)
    private int dialog_length;      //délka dialogu
    private bool writing = false;       //průběh psaní
    private bool done = false;      //dokončeno

    public bool dialogDone = false;

    public TextAsset textFile;

    private Canvas pausecanvas;

    void Start()
    {
        canvas.enabled = true;
        dialog_script = GetComponent<Dialog>();     //skript

        string text = textFile.text;
        array = text.Split("\n"[0]);        //text na řádky

        dialog_length = array.Length - 1;

        pfp_object.sprite = profile_picture;        //obrázek
        dialog_name.color = name_color;     //barva jména
        dialog_name.text = array[0];       //jméno

        dialog_position.text = "[" + position + "/" + dialog_length + "]";

        writing = true;
        //StartCoroutine(AnimateText(array[position]));       //první věta dialogu
        StartCoroutine(AnimateTextSound(array[position], dialog_box));
    }

    public void Refresh()
    {
        string text = textFile.text;
        array = text.Split("\n"[0]);        //text na řádky
        dialog_length = array.Length - 1;
        position = 1;
        dialog_position.text = "[" + position + "/" + dialog_length + "]";
        pfp_object.sprite = profile_picture;        //obrázek
        dialog_name.color = name_color;     //barva jména
        dialog_name.text = array[0];       //jméno
        dialogDone = false;
        StartCoroutine(AnimateTextSound(array[1], dialog_box));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && writing == false)     //pokračování dialogu
        {
            if (position == array.Length - 1 && done == true)       //konec/deaktivování
            {
                //zakázání DialogCanvas
                writing = true;
                canvas.GetComponent<Canvas>().enabled = false;
                dialog_script.enabled = false;
                Debug.Log("Canvas and Dialog script disabled.");
            }

            if (position < array.Length - 1)        //přičtení
            {
                writing = true;
                done = true;
                position++;
                dialog_position.text = "[" + position + "/" + dialog_length + "]";
                //StartCoroutine(AnimateText(array[position]));
                StartCoroutine(AnimateTextSound(array[position], dialog_box));
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && writing == false)        //přetáčení dialogu
        {
            if (position > 1)
            {
                writing = true;
                position--;
                dialog_position.text = "[" + position + "/" + dialog_length + "]";
                //StartCoroutine(AnimateText(array[position]));
                StartCoroutine(AnimateTextSound(array[position], dialog_box));
            }
        }

        //konec
        if (position == dialog_length && dialogDone == false)
            dialogDone = true;
    }

    IEnumerator AnimateText(string strComplete)     //https://answers.unity.com/questions/1495076/waitforseconds-with-small-values-seems-to-be-very.html
    {
        int i = 0;
        dialog_text.text = "";
        while (i < strComplete.Length)
        {
            dialog_text.text += strComplete[i++];
            yield return null;
        }

        writing = false;
    }

    IEnumerator AnimateTextSound(string strComplete, GameObject audio)
    {
        int i = 0;
        dialog_text.text = "";

        while (i < strComplete.Length)
        {
            char letter = strComplete[i++];
            dialog_text.text += letter;

            if (letter != ' ')
                PlaySound(audio.GetComponent<AudioSource>(), "Zvuky/sfx/Dialog/" + profile_picture.name + "/" + profile_picture.name + Random.Range(1, 5), 0.2F, false);

            yield return null;
        }

        writing = false;
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

        //přehrání
        audio.Play();
    }
}
