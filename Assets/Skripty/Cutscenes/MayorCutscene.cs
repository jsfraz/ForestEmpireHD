using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MayorCutscene : MonoBehaviour
{
    public GameObject player;      //PlayerController
    public Canvas canvas;     //DialogCanvas
    public GameObject mayor;

    public Canvas gameInfoCanvas;

    private Dialog dialog_script;
    private MayorCutscene mayor_cutscene;

    public Canvas canvasObjective;
    public TextAsset dialogText;
    public TextAsset quest;

    private bool zakonceno = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        //zakázání pohybu
        player.GetComponent<PlayerController>().enabled = false;

        canvas.GetComponent<Canvas>().enabled = true;
        gameInfoCanvas.enabled = false;
        mayor.SetActive(true);

        //Dialog
        dialog_script = GameObject.Find("Main Camera").GetComponent<Dialog>();
        dialog_script.text_file = dialogText.text;
        //Resources.Load will search for a directory in Assets/Resources https://stackoverflow.com/questions/24977986/why-does-resources-load-sprite-return-null
        string profile_picture = "mayor";
        dialog_script.profile_picture = Resources.Load<Sprite>("Textury/Itemy/Profilovky/" + profile_picture);
        dialog_script.name_color = Color.green;
        dialog_script.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        //konec
        if (dialog_script.dialogDone && zakonceno == false)
        {
            zakonceno = true;

            canvas.enabled = false;
            gameInfoCanvas.enabled = true;
            player.GetComponent<PlayerController>().enabled = true;
            mayor.GetComponent<Animator>().Play("Run");
            PlaySound(mayor.GetComponent<AudioSource>(), "Zvuky/sfx/run", 0.3F, false);
            StartCoroutine(MayorRun());
            canvasObjective.GetComponent<ObjectiveText>().DisplayObjective(quest.text, 10F);

            //statistiky hráče
            string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ForestEmpireHD";
            if (Directory.Exists(saveFolder) == false)
                Directory.CreateDirectory(saveFolder);
            string[] stats = File.ReadAllLines(saveFolder + "/stats");
            stats[7] = "false";      //true = spuštění poprvé, false = spuštění není poprvé

            File.WriteAllLines(saveFolder + "/stats", stats);
        }
    }

    IEnumerator MayorRun()
    {
        for (int i = 0; i < 500 + 25; i += 1)
        {
            mayor.GetComponent<Transform>().Translate(-0.03F, 0.14F, 0);
            yield return null;
        }

        Destroy(mayor);
    }

    void PlaySound(AudioSource audio, string resource, float volume, bool loop)
    {
        audio.volume = volume;
        audio.clip = Resources.Load(resource) as AudioClip;     //inicializuje zvukový klip

        //nastavení loopování
        if (loop)
            audio.loop = true;
        else
            audio.loop = false;

        //přehrání
        audio.Play();
    }
}
