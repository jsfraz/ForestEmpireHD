using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public Text creditsText;
    public TextAsset textCategories;
    public TextAsset textPeople;
    public TextAsset textBetaTesters;
    public TextAsset textEnd;

    private string[] cathegories;
    private string[] people;
    private string[] beta;

    // Start is called before the first frame update
    void Start()
    {
        creditsText.text = "";
        cathegories = textCategories.text.Split("\n"[0]);
        people = textPeople.text.Split("\n"[0]);
        beta = textBetaTesters.text.Split("\n"[0]);

        StartCoroutine(DisplayCredits());
    }

    private IEnumerator DisplayCredits()
    {
        //kategorie lidí
        creditsText.text = cathegories[0];
        yield return new WaitForSeconds(4.5F);
        
        //lidi
        for (int i = 0; i < people.Length; i++)
        {
            creditsText.text = people[i];
            yield return new WaitForSeconds(4F);
            creditsText.text = "";
            yield return new WaitForSeconds(1.2F);
        }

        //kategorie betatesterù
        creditsText.text = cathegories[1];
        yield return new WaitForSeconds(4.5F);

        //betatesters
        for (int i = 0; i < beta.Length; i++)
        {
            creditsText.text = beta[i];
            yield return new WaitForSeconds(4F);
            creditsText.text = "";
            yield return new WaitForSeconds(1.2F);
        }

        //konec
        creditsText.text = textEnd.text;
        yield return new WaitForSeconds(5F);
        GetComponent<AudioSource>().Stop();
        GetComponent<SceneChanger>().ChangeScene();
    }
}
