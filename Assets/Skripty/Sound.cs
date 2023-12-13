using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public static void PlaySound(AudioSource audio, string resource, float volume, bool loop)
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

    public static IEnumerator StopSound(AudioSource audio)
    {
        for (float i = audio.volume; i > 0; i -= 0.01F)
        {
            audio.volume = i;
            yield return new WaitForSeconds(0.05F);
        }

        audio.Stop();
    }
}
