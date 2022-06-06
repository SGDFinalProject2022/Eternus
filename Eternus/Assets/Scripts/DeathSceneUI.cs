using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathSceneUI : MonoBehaviour
{
    [SerializeField] List<string> sentences = new List<string>();
    [SerializeField] Text crypticText;
    [SerializeField] Color highlightColor;
    [SerializeField] Color dehighlightColor;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        SelectRandomSentence();
    }

    void SelectRandomSentence()
    {
        int random = Random.Range(0, sentences.Count);
        crypticText.text = sentences[random];
    }

    public void Retry()
    {
        SaveData data = SaveLoad.Load();
        GlobalData.instance.LoadScene(data.scene);
    }

    public void MainMenu()
    {
        GlobalData.instance.LoadScene("MainMenu");
    }

    public void HighlightText(Text text)
    {
        text.color = highlightColor;
    }

    public void DehighlightText(Text text)
    {
        text.color = dehighlightColor;
    }
}
