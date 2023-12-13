using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Cat : MonoBehaviour
{
    public Camera cmr;
    public GameObject player;
    public TextAsset dialogCat;
    public Sprite pfp;
    private Dialog dialog_script;

    private bool collide = false;
    private bool visible = false;
    private bool zakonceno = false;

    private string[] array;
    private string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ForestEmpireHD";

    // Start is called before the first frame update
    void Start()
    {
        dialog_script = cmr.GetComponent<Dialog>();
        array = File.ReadAllLines(saveFolder + "/events/oldlady");

        if (array[0] == "true" && array[1] == "true")
            gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (collide && Input.GetKeyDown(KeyCode.E) && player.GetComponent<PlayerController>().enabled)
        {
            zakonceno = false;

            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<Animator>().SetFloat("Horizontal", 0F);
            player.GetComponent<Animator>().SetFloat("Vertical", 0F);

            dialog_script.enabled = true;
            dialog_script.textFile = dialogCat;
            dialog_script.profile_picture = pfp;
            dialog_script.name_color = Color.green;
            dialog_script.canvas.enabled = true;
            if (dialog_script.dialogDone == true)
                dialog_script.Refresh();
        }

        //konec
        if (Input.GetKeyDown(KeyCode.Return) && zakonceno == false && dialog_script.dialogDone)
        {
            zakonceno = true;
            array[0] = "true";
            File.WriteAllLines(saveFolder + "/events/oldlady", array);
            player.GetComponent<PlayerController>().enabled = true;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.name == "Hero")
        {
            collide = true;
            if (visible == false)
            {
                visible = true;
                StartCoroutine(LetterFadeIn());
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Hero")
        {
            collide = false;
            if (visible == true)
            {
                visible = false;
                StartCoroutine(LetterFadeOut());
            }
        }
    }

    private IEnumerator LetterFadeIn()
    {
        SpriteRenderer letter_renderer = GameObject.Find("InteractLetter").GetComponent<SpriteRenderer>();

        for (byte i = 0; i < 250; i += 25)
        {
            yield return null;
            letter_renderer.color = new Color32(255, 255, 200, i);
        }

        letter_renderer.color = new Color32(255, 255, 200, 255);
    }

    private IEnumerator LetterFadeOut()
    {
        SpriteRenderer letter_renderer = GameObject.Find("InteractLetter").GetComponent<SpriteRenderer>();

        for (byte i = 250; i > 0; i -= 25)
        {
            yield return null;
            letter_renderer.color = new Color32(255, 255, 200, i);
        }

        letter_renderer.color = new Color32(255, 255, 200, 0);
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
