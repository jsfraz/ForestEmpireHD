using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSceneChanger : MonoBehaviour
{
    public Color loadToColor = Color.black;     //barva
    public string sceneName;        //Scene
    public bool sound = false;
    public float delay = 0;

    private void OnTriggerEnter2D(Collider2D collision)     //kolize s hráčem
    {
        if (collision.name == "Hero")
        {
            if (sound)
                StartCoroutine(ChangeWithSound());
            else
                StartCoroutine(Change());
        }
    }

    private IEnumerator Change()
    {
        PlayerController player = GameObject.Find("Hero").GetComponent<PlayerController>();
        if (player != null)
            player.enabled = false;

        yield return new WaitForSeconds(delay);
        Debug.Log("Changing scene to: " + sceneName + ".");
        Initiate.Fade(sceneName, loadToColor, 3f);      //Fade script
    }

    private IEnumerator ChangeWithSound()
    {
        PlayerController player = GameObject.Find("Hero").GetComponent<PlayerController>();
        if (player != null)
            player.enabled = false;

        PlaySound(GetComponent<AudioSource>(), "Zvuky/sfx/plesk", 0.5F, false);
        yield return new WaitForSeconds(delay);
        Debug.Log("Changing scene to: " + sceneName + ".");
        Initiate.Fade(sceneName, loadToColor, 3f);      //Fade script
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
