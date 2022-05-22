using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Controls the UI for the player
/// </summary>
public class UI : MonoBehaviour
{
    [Header("Text")]
    public Text interactText; //text below the crosshair
    public Text objectiveText; //text on the top of the screen
    [Header("Objectives")]
    public float objectiveShowDelay = 0f;
    public float objectiveHideDelay = 3f;
    [Header("Items")]
    public Text itemText;
    public Image itemImage;
    public Sprite defaultItemImage;
    
    bool isObjectiveRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        objectiveText.color = new Color(objectiveText.color.r, objectiveText.color.g, objectiveText.color.b, 0f);
        itemImage.color = new Color(objectiveText.color.r, objectiveText.color.g, objectiveText.color.b, 0f);
        itemText.color = new Color(objectiveText.color.r, objectiveText.color.g, objectiveText.color.b, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowObjective(string objective)
    {
        StartCoroutine(ShowObjectiveCoroutine(objective));
    }

    public void ShowItem(Sprite item)
    {
        StartCoroutine(ShowItemCoroutine(item));
    }

    public void HideItem()
    {
        StartCoroutine(HideItemCoroutine());
    }

    //keeping just in case we need it later (broken)
    public IEnumerator AlphaFade(Color input)
    {
        //float alpha = input.a;
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


    IEnumerator ShowObjectiveCoroutine(string objective)
    {
        if (isObjectiveRunning)
        {
            yield break;
        }

        isObjectiveRunning = true;
        yield return new WaitForSeconds(0); //0 or 1
        objectiveText.text = objective;
        for (float t = 0.0f; t < 1.0f; t += 0.01f)
        {
            objectiveText.color = new Color(objectiveText.color.r, objectiveText.color.g, objectiveText.color.b, t);
            yield return null;
        }
        yield return new WaitForSeconds(3); //5
        for (float t = 1.0f; t > 0.0f; t -= 0.01f)
        {
            objectiveText.color = new Color(objectiveText.color.r, objectiveText.color.g, objectiveText.color.b, t);
            yield return null;
        }
        isObjectiveRunning = false;
    }

    IEnumerator ShowItemCoroutine(Sprite item)
    {
        itemImage.sprite = item;
        for (float t = 0.0f; t < 1.0f; t += 0.005f)
        {
            itemImage.color = new Color(objectiveText.color.r, objectiveText.color.g, objectiveText.color.b, t);
            itemText.color = new Color(objectiveText.color.r, objectiveText.color.g, objectiveText.color.b, t);
            yield return null;
        }
    }
    IEnumerator HideItemCoroutine()
    {
        for (float t = 1.0f; t > 0.0f; t -= 0.005f)
        {
            itemImage.color = new Color(objectiveText.color.r, objectiveText.color.g, objectiveText.color.b, t);
            itemText.color = new Color(objectiveText.color.r, objectiveText.color.g, objectiveText.color.b, t);
            yield return null;
        }
        itemImage.sprite = defaultItemImage;
    }
}
