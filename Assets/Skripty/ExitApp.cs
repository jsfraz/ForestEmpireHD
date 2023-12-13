using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitApp : MonoBehaviour
{
    public void Exit()
    {
        Debug.Log("Quitting.");
        Application.Quit();
    }
}
