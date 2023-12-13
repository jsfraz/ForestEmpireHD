using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LoadGame : MonoBehaviour
{
    private string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ForestEmpireHD";

    public void LoadSavedGame()
    {
        bool save = true;

        if (File.Exists(saveFolder + "/stats") == false)
            save = false;
        if (File.Exists(saveFolder + "/village_buildings") == false)
            save = false;
        if (File.Exists(saveFolder + "/inventory") == false)
            save = false;
        if (File.Exists(saveFolder + "/position") == false)
            save = false;

        if (save)
            GetComponent<SceneChanger>().ChangeScene();
    }
}
