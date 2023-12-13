using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HarildaDoorE : MonoBehaviour
{
    public Camera cmr;
    public GameObject harilda;
    public GameObject player;
    public TextAsset notLevel;
    public TextAsset alreadyDead;
    public TextAsset harildaDialog;
    public Sprite harildaPfp;

    private Dialog dialog_script;

    private bool collide = false;
    private bool visible = false;

    private string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ForestEmpireHD";
    string[] lastfight;

    private void Start()
    {
        dialog_script = cmr.GetComponent<Dialog>();
        lastfight = File.ReadAllLines(saveFolder + "/lastfight");
    }

    void Update()
    {
        if (collide && Input.GetKeyDown(KeyCode.E))
        {
            if (GameObject.Find("Village").GetComponent<ManageVillage>().player.level >= 3 && lastfight[0] == "false")
            {
                //cutsc�na
                GameObject.Find("Main Camera").GetComponent<AudioSource>().Stop();
                harilda.SetActive(true);
                player.GetComponent<PlayerController>().enabled = false;
                player.GetComponent<Animator>().SetFloat("Horizontal", 0F);
                player.GetComponent<Animator>().SetFloat("Vertical", 0F);
                PlaySound(harilda.GetComponent<AudioSource>(), "Zvuky/sfx/thunder", 0.6F, false);

                dialog_script.textFile = harildaDialog;
                dialog_script.profile_picture = harildaPfp;
                dialog_script.name_color = Color.red;
                GameObject.Find("DialogCanvas").GetComponent<Canvas>().enabled = true;
                GameObject.Find("InteractLetter").GetComponent<SpriteRenderer>().enabled = false;
                if (dialog_script.dialogDone == true)
                    dialog_script.Refresh();
                dialog_script.enabled = true;
            }     
            else
            {
                ObjectiveText objective = GameObject.Find("CanvasObjective").GetComponent<ObjectiveText>();

                if (GameObject.Find("Village").GetComponent<ManageVillage>().player.level < 3)
                    objective.DisplayObjective(notLevel.text, 8F);
                if (lastfight[0] == "true")
                    objective.DisplayObjective(alreadyDead.text, 8F);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && dialog_script.dialogDone && collide)
            GetComponent<SceneChanger>().ChangeScene();
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
        if (collision.gameObject.name == "Hero" && !dialog_script.dialogDone)
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
