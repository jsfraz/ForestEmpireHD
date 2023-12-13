using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldMan : MonoBehaviour
{
    public TextAsset oldManText;
    public Sprite oldManPfp;
    private GameObject cmr;
    private Dialog dialog;

    private bool collide = false;
    private bool visible = false;
    private bool zakonceno = false;

    private void Start()
    {
        cmr = GameObject.Find("Main Camera");
        dialog = cmr.GetComponent<Dialog>();
    }

    void Update()
    {
        if (collide && Input.GetKeyDown(KeyCode.E) && GameObject.Find("Hero").GetComponent<PlayerController>().enabled)
        {
            zakonceno = false;

            GameObject player = GameObject.Find("Hero");
            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<Animator>().SetFloat("Horizontal", 0F);
            player.GetComponent<Animator>().SetFloat("Vertical", 0F);

            dialog.enabled = true;
            dialog.textFile = oldManText;
            dialog.profile_picture = oldManPfp;
            dialog.name_color = Color.green;
            dialog.canvas.enabled = true;
            if (dialog.dialogDone == true)
                dialog.Refresh();
        }

        //konec
        if (Input.GetKeyDown(KeyCode.Return) && zakonceno == false && dialog.dialogDone)
        {
            zakonceno = true;
            //dialog.dialogDone = false;
            GameObject.Find("Hero").GetComponent<PlayerController>().enabled = true;
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
}
