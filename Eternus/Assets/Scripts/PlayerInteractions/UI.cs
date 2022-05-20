using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Controls the UI for the player
/// </summary>
public class UI : MonoBehaviour
{
    public Text interactText;
    public Text objectiveText;

    // Start is called before the first frame update
    void Start()
    {
        //AlphaSet(objectiveText.color, 0f);
        //StartCoroutine(ShowObjective("Objective: Push all the buttons"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sets an color's alpha to a value over time (broken right now)
    /// </summary>
    /// <param name="input"></param>
    /// <param name="value"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator AlphaFade(Color input, float value, float time)
    {
        float alpha = input.a;
        /*for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
        {
            input = new Color(input.r, input.g, input.b, Mathf.Lerp(alpha, value, t));
            yield return null;
            print(input.a);
        }*/
        for (float t = 0.0f; t < 1.0f; t += 0.01f)
        {
            input = new Color(input.r, input.g, input.b, t);
            yield return null;
        }
    }

    /// <summary>
    /// Sets an color's alpha to a value
    /// </summary>
    /// <param name="input"></param>
    /// <param name="value"></param>
    public void AlphaSet(Color input, float value) { input = new Color(input.r, input.g, input.b, value); }

    public IEnumerator ShowObjective(string objective)
    {
        objectiveText.text = objective;
        StartCoroutine(AlphaFade(objectiveText.color, 1f, 1f));
        yield return new WaitForSeconds(5);
        StartCoroutine(AlphaFade(objectiveText.color, 0.5f, 1f));
    }
}
