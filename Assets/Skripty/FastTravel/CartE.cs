using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartE : MonoBehaviour
{
    public Canvas CanvasFastTravel;
    public string location;

    private bool collide = false;
    private bool visible = false;

    void Update()
    {
        if (collide && Input.GetKeyDown(KeyCode.E) && GameObject.Find("Hero").GetComponent<PlayerController>().enabled && GameObject.Find("InventoryCanvas").GetComponent<Canvas>().enabled == false)
            CanvasFastTravel.GetComponent<FastTravel>().Show(location);
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
}
