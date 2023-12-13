using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{
    public GameObject player;      //PlayerController
    public Canvas canvas;     //DialogCanvas
    
    private Dialog dialog_script;
    private SceneChanger scene_changer;

    //GameObject.Find("nazev");

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Beginning of first cutscene.");

        scene_changer = GetComponent<SceneChanger>();

        //animace a zakázání pohybu
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Animator>().Play("Downed");
        Debug.Log("Hero movement disabled, playing animation.");

        //povolení DialogCanvas
        if (canvas.GetComponent<Canvas>().enabled == false)
        {
            canvas.GetComponent<Canvas>().enabled = true;
            Debug.Log("Canvas enabled.");
        }

        //Dialog
        dialog_script = GetComponent<Dialog>();
        dialog_script.text_file = "firstHarilda";
        //Resources.Load will search for a directory in Assets/Resources https://stackoverflow.com/questions/24977986/why-does-resources-load-sprite-return-null
        string profile_picture = "harilda";
        dialog_script.profile_picture = Resources.Load<Sprite>("Textury/Itemy/Profilovky/" + profile_picture);
        dialog_script.name_color = Color.red;
        dialog_script.enabled = true;
    }

    //konec
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && dialog_script.dialogDone)
        {
            scene_changer.sceneName = "FirstFight";
            scene_changer.ChangeScene();
        }
    }
}
