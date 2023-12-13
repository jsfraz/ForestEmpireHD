using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OldLady : MonoBehaviour
{
    public Camera cmr;
    public GameObject player;
    public TextAsset dialogFirst;
    public TextAsset dialogFound;
    public TextAsset dialogThanks;
    public Sprite oldLadyPfp;

    private Dialog dialog_script;

    private bool collide = false;
    private bool visible = false;
    private bool zakonceno = false;

    private string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ForestEmpireHD";
    //prvn� bool znamen�, jestli je ko�ka nalezen�, druh� se pou��v� na d�kov�n�
    private string[] array;

    private void Start()
    {
        dialog_script = cmr.GetComponent<Dialog>();
    }

    private void Update()
    {
        if (collide && Input.GetKeyDown(KeyCode.E) && player.GetComponent<PlayerController>().enabled)
        {
            zakonceno = false;

            array = File.ReadAllLines(saveFolder + "/events/oldlady");

            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<Animator>().SetFloat("Horizontal", 0F);
            player.GetComponent<Animator>().SetFloat("Vertical", 0F);

            if (array[0] == "false")       //prosba
                dialog_script.textFile = dialogFirst;
            if (array[0] == "true" && array[1] == "true")     //pod�kov�n�
                dialog_script.textFile = dialogThanks;
            if (array[0] == "true" && array[1] == "false")        //sd�len� kde je
            {
                dialog_script.textFile = dialogFound;
                array[1] = "true";
                File.WriteAllLines(saveFolder + "/events/oldlady", array);

                //p�i�ten� pen�z
                ManageVillage village = GameObject.Find("Village").GetComponent<ManageVillage>();
                village.player.money += 300;
                StartCoroutine(village.Save());
                StartCoroutine(village.LoadGameInfo());
            }

            dialog_script.enabled = true;
            dialog_script.profile_picture = oldLadyPfp;
            dialog_script.name_color = Color.green;
            dialog_script.canvas.enabled = true;
            if (dialog_script.dialogDone == true)
                dialog_script.Refresh();
        }

        //konec
        if (Input.GetKeyDown(KeyCode.Return) && zakonceno == false && dialog_script.dialogDone)
        {
            zakonceno = true;
            //dialog_script.dialogDone = false;
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
        audio.clip = Resources.Load(resource) as AudioClip;     //inicializuje zvukov� klip

        //nastaven� loopov�n�
        if (loop)
            audio.loop = true;
        else
            audio.loop = false;

        //p�ehr�n�
        audio.Play();
    }
}
