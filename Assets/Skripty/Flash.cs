using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    public GameObject objekt;
    private SpriteRenderer sprite;

    //flash
    IEnumerator Flashing()
    {
        for (byte i = 240; i > 0; i -= 20)
        {
            sprite.color = new Color32(255, i, i, 255);

            yield return null;
        }
        for (byte i = 0; i < 240; i += 20)
        {
            sprite.color = new Color32(255, i, i, 255);

            yield return null;
        }

        sprite.color = new Color32(255, 255, 255, 255);
    }

    //start flashnutí
    public void TriggerFlash()
    {
        sprite = objekt.GetComponent<SpriteRenderer>();
        StartCoroutine(Flashing());
    }
}
