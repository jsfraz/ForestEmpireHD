using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightButtonsPress : MonoBehaviour
{
    public Camera cmr;

    //kontroluje, které tlačítko je zmáčknuté
    public void Pressed()
    {
        if (cmr.GetComponent<Fight>().writing == false)
        {
            Button btn = GetComponent<Button>();

            if (btn.name == "ButtonFight")
                cmr.GetComponent<Fight>().fightPressed = true;

            if (btn.name == "ButtonInventory")
                cmr.GetComponent<Fight>().inventoryPressed = true;

            if (btn.name == "ButtonMercy")
                cmr.GetComponent<Fight>().mercyPressed = true;

            if (btn.name == "ButtonRun")
                cmr.GetComponent<Fight>().runPressed = true;
        }
    }
}
