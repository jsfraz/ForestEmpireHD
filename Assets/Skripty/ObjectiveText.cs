using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveText : MonoBehaviour
{
    public Text objectiveText;
    private bool writing = false;

    private void Start()
    {
        objectiveText.text = "";
    }

    public void DisplayObjective(string text, float time)
    {
        if (writing == false)
        {
            writing = true;
            StartCoroutine(LetterFadeing(time));
            StartCoroutine(AnimateText(objectiveText, text));
        }
    }

    private IEnumerator AnimateText(Text messageObject, string text)        //animování textu
    {
        int i = 0;
        messageObject.text = "";

        while (i < text.Length)
        {
            messageObject.text += text[i++];
            yield return null;
        }
    }

    private IEnumerator LetterFadeing(float delay)
    {
        for (byte i = 0; i < 250; i += 25)
        {
            yield return null;
            objectiveText.color = new Color32(255, 255, 255, i);
        }
        objectiveText.color = new Color32(255, 255, 255, 255);

        yield return new WaitForSeconds(delay);
        for (byte i = 250; i > 0; i -= 25)
        {
            yield return null;
            objectiveText.color = new Color32(255, 255, 200, i);
        }
        objectiveText.color = new Color32(255, 255, 200, 0);

        writing = false;
    }
}
