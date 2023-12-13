using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Color loadToColor = Color.black;
    public string sceneName;

    public void ChangeScene()
    {
        Initiate.Fade(sceneName, loadToColor, 1f);
        Debug.Log("Changing scene to: " + sceneName + ".");
    }
}