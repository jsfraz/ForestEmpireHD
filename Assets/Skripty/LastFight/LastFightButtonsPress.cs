using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LastFightButtonsPress : MonoBehaviour
{
    public Camera cmr;

    //kontroluje, které tlačítko je zmáčknuté
    public void Pressed()
    {
        if (cmr.GetComponent<LastFight>().writing == false)
        {
            Button btn = GetComponent<Button>();

            if (btn.name == "ButtonFight")
                cmr.GetComponent<LastFight>().fightPressed = true;

            if (btn.name == "ButtonInventory")
                cmr.GetComponent<LastFight>().inventoryPressed = true;

            if (btn.name == "ButtonMercy")
                cmr.GetComponent<LastFight>().mercyPressed = true;

            if (btn.name == "ButtonRun")
                cmr.GetComponent<LastFight>().runPressed = true;
        }
    }
}
