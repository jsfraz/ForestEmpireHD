using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cart : MonoBehaviour
{
    public Canvas travelCanvas;
    public GameObject player;

    public Vector3 cartVillagePosition;
    public Vector3 cartCavePosition;
    public Vector3 cartHousePosition;
    public Vector3 cartPondPosition;

    public void Travel(string currentLocation, string destination)
    {
        StartCoroutine(Black());
        StartCoroutine(Animation(currentLocation, destination));
    }

    private IEnumerator Animation(string currentLocation, string destination)
    {
        //ztmavení
        for (byte i = 0; i < 250; i += 25)
        {
            travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }
        travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 255);

        //nastavení sorting layerù, spritù atd
        float increment = 0.01F;
        Vector3 firstCartNewPosition = GetComponent<Transform>().position;
        Vector3 secondCartNewPosition;
        GameObject secondCart = null;
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Animator>().Play("FrontHalf");
        player.transform.Find("InteractLetter").GetComponent<SpriteRenderer>().color = new Color32(255, 255, 200, 0);
        transform.Find("cart_bottom").GetComponent<SpriteRenderer>().sortingLayerName = "Itemy";
        player.GetComponent<Transform>().position = firstCartNewPosition;

        //odtmavení
        for (byte i = 250; i > 0; i -= 25)
        {
            travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }
        travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

        //inicializace druhého vozíku
        if (destination == "village")
            secondCart = GameObject.Find("cart_village");
        if (destination == "cave")
            secondCart = GameObject.Find("cart_cave");
        if (destination == "house")
            secondCart = GameObject.Find("cart_house");
        if (destination == "pond")
            secondCart = GameObject.Find("cart_pond");

        //animace jízdy
        PlaySound(GetComponent<AudioSource>(), "Zvuky/sfx/engineStart", 0.5F, false);
        secondCartNewPosition = secondCart.GetComponent<Transform>().position;
        for (int i = 0; i < 120; i++)
        {
            yield return new WaitForSeconds(0.01F);

            GetComponent<Transform>().position = firstCartNewPosition;
            secondCart.GetComponent<Transform>().position = secondCartNewPosition;
            player.GetComponent<Transform>().position = firstCartNewPosition;

            firstCartNewPosition.x += increment;
            secondCartNewPosition.x += increment;
            increment *= 1.02F;
        }
        transform.Find("cart_bottom").GetComponent<SpriteRenderer>().sortingLayerName = "vozik";

        //teleport, zvuky, nastavení sorting layerù atd
        PlaySound(travelCanvas.GetComponent<AudioSource>(), "Zvuky/sfx/engineGo", 0.5F, false);
        yield return new WaitForSeconds(3F);
        player.transform.Find("InteractLetter").GetComponent<SpriteRenderer>().enabled = false;
        player.GetComponent<Transform>().position = secondCart.GetComponent<Transform>().position;
        secondCart.transform.Find("cart_bottom").GetComponent<SpriteRenderer>().sortingLayerName = "Itemy";

        firstCartNewPosition = GetComponent<Transform>().position;
        secondCartNewPosition = secondCart.GetComponent<Transform>().position;
        increment = 0.1076514F;

        //animace zastavení
        PlaySound(secondCart.GetComponent<AudioSource>(), "Zvuky/sfx/engineStop", 0.5F, false);
        for (int i = 0; i < 120; i++)
        {
            yield return new WaitForSeconds(0.01F);

            GetComponent<Transform>().position = firstCartNewPosition;
            secondCart.GetComponent<Transform>().position = secondCartNewPosition;
            player.GetComponent<Transform>().position = secondCartNewPosition;

            firstCartNewPosition.x -= increment;
            secondCartNewPosition.x -= increment;
            increment /= 1.02F;
        }

        //ztmavení
        yield return new WaitForSeconds(1F);
        for (byte i = 0; i < 250; i += 25)
        {
            travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }
        travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 255);

        //zarovnání vozíkù
        if (currentLocation == "village")
            GetComponent<Transform>().position = cartVillagePosition;
        if (currentLocation == "cave")
            GetComponent<Transform>().position = cartCavePosition;
        if (currentLocation == "house")
            GetComponent<Transform>().position = cartHousePosition;
        if (destination == "village")
            secondCart.GetComponent<Transform>().position = cartVillagePosition;
        if (destination == "cave")
            secondCart.GetComponent<Transform>().position = cartCavePosition;
        if (destination == "house")
            secondCart.GetComponent<Transform>().position = cartHousePosition;
        if (destination == "pond")
            secondCart.GetComponent<Transform>().position = cartPondPosition;

        //nastavení sorting layerù a konec animace atd
        secondCart.transform.Find("cart_bottom").GetComponent<SpriteRenderer>().sortingLayerName = "vozik";
        secondCartNewPosition.y -= 1;
        player.transform.Find("InteractLetter").GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<Transform>().position = secondCartNewPosition;
        player.GetComponent<PlayerController>().enabled = true;
        player.GetComponent<Animator>().Play("IdleFront");
        StartCoroutine(GameObject.Find("Village").GetComponent<ManageVillage>().Save());

        //odtmavení
        for (byte i = 250; i > 0; i -= 25)
        {
            travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return null;
        }
        travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

        //fix že nejde otevøít inventáø
        GameObject.Find("CanvasFastTravel").GetComponent<Canvas>().enabled = false;
    }

    private IEnumerator Black()
    {
        yield return new WaitForSeconds(0.75F);

        for (byte i = 0; i < 250; i += 25)
        {
            travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return new WaitForSeconds(0.1F);
        }
        travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 255);

        yield return new WaitForSeconds(4F);

        for (byte i = 250; i > 0; i -= 25)
        {
            travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, i);
            yield return new WaitForSeconds(0.1F);
        }
        travelCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
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
