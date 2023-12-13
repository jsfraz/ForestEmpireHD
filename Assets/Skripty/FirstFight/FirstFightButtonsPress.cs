using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstFightButtonsPress : MonoBehaviour
{
    public Camera cmr;

    //kontroluje, které tlačítko je zmáčknuté
    public void Pressed()
    {
        if (cmr.GetComponent<FirstFight>().writing == false)
        {
            Button btn = GetComponent<Button>();

            if (btn.name == "ButtonFight")
                cmr.GetComponent<FirstFight>().fightPressed = true;

            if (btn.name == "ButtonInventory")
                cmr.GetComponent<FirstFight>().inventoryPressed = true;

            if (btn.name == "ButtonMercy")
                cmr.GetComponent<FirstFight>().mercyPressed = true;

            if (btn.name == "ButtonRun")
                cmr.GetComponent<FirstFight>().runPressed = true;
        }
    }
}
