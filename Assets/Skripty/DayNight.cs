using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class DayNight : MonoBehaviour
{
    public Text timeText;

    public int time;
    public int minutes = 0;
    public int hours = 0;
    public float intesity;

    private Canvas pauseCanvas;
    private UnityEngine.Rendering.Universal.Light2D dayNight;

    private string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ForestEmpireHD";

    // Start is called before the first frame update
    void Start()
    {
        pauseCanvas = GameObject.Find("CanvasPause").GetComponent<Canvas>();
        dayNight = GetComponent<UnityEngine.Rendering.Universal.Light2D>();

        StartCoroutine(Time());
        DisplayTime();
    }

    private void Update()
    {
        intesity = dayNight.intensity;

        //CHEAT
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (time < 490 || time > 1080)
            {
                time = 491;
                dayNight.intensity = 1;
            }
            DisplayTime();
        }
    }

    private void GetTime()
    {
        string[] timesave = File.ReadAllLines(saveFolder + "/time");
        time = int.Parse(timesave[0]);
        dayNight.intensity = float.Parse(timesave[1]);
    }

    /*
    private IEnumerator SaveTime()
    {
        yield return new WaitForEndOfFrame();
        string[] timesave = { time.ToString() };
        File.WriteAllLines(saveFolder + "/time", timesave);
    }
    */

    private IEnumerator Time()
    {
        GetTime();

        while (true)
        {
            //yield return new WaitForSeconds(1F);
            yield return new WaitForSeconds(0.75F);

            if (pauseCanvas.enabled == false)
            {
                //p�i��t�n� �asu
                time++;

                if (time > 350 && time <= 490)
                    dayNight.intensity += 0.0064285714285714F;
                if (time > 1080 && time <= 1220)
                    dayNight.intensity -= 0.0064285714285714F;
                if (time == 1440)
                    time = 0;

                DisplayTime();
            }
        }
    }

    //vypisov�n� �asu
    private void DisplayTime()
    {
        hours = time / 60;
        minutes = time - (hours * 60);

        string hoursString = hours.ToString();
        string minutesString = minutes.ToString();

        if (hoursString.Length == 1)
            hoursString = "0" + hoursString;
        if (minutesString.Length == 1)
            minutesString = "0" + minutesString;

        timeText.text = "Čas: " + hoursString + ":" + minutesString;
    }
}
