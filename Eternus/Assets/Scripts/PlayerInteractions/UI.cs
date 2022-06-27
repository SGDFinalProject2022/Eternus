using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    [Header("Pause")]
    [SerializeField] GameObject pauseMenuPanel;
    [HideInInspector] public bool isPaused;
    public HeadBobController headBobController;
    [Header("Tutorial")]
    public GameObject tutorialPanel;
    [SerializeField] Image tutImage;
    [SerializeField] Text tutorialText;
    
    bool isObjectiveRunning = false;
    [HideInInspector] public bool panelIsOpen;
    [Header("Other")]
    public PlayerMovement move;
    public MouseLook look;

    // Start is called before the first frame update
    void Start()
    {
        objectiveText.color = new Color(objectiveText.color.r, objectiveText.color.g, objectiveText.color.b, 0f);
        itemImage.color = new Color(objectiveText.color.r, objectiveText.color.g, objectiveText.color.b, 0f);
        itemText.color = new Color(objectiveText.color.r, objectiveText.color.g, objectiveText.color.b, 0f);
        pauseMenuPanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!panelIsOpen && Input.GetButtonDown("Pause"))
        {
            PauseGame(!isPaused);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && Input.GetAxis("Crouch") > 0) { PauseGame(true); }
    }

    void PauseGame(bool pause)
    {
        Debug.Log(pause);
        headBobController.enableHeadbob = !pause;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = pause ? 0.001f : 1;
        pauseMenuPanel.SetActive(pause);
        isPaused = pause;
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

    public void ShowTutorial(string tutorial)
    {
        StartCoroutine(ShowTutorialCoroutine(tutorial));
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

    IEnumerator ShowTutorialCoroutine(string tutorial)
    {
        tutorialPanel.SetActive(true);
        tutorialText.text = tutorial;
        Color tutPanelColor = tutImage.color;
        for (float t = 0.0f; t < 1.0f; t += 0.01f)
        {
            tutPanelColor = new Color(tutPanelColor.r, tutPanelColor.g, tutPanelColor.b, t);
            tutorialText.color = new Color(tutorialText.color.r, tutorialText.color.b, tutorialText.color.g, t);
            yield return null;
        }
        yield return new WaitForSeconds(5);
        for (float t = 1.0f; t > 0.0f; t -= 0.01f)
        {
            tutPanelColor = new Color(tutPanelColor.r, tutPanelColor.g, tutPanelColor.b, t);
            tutorialText.color = new Color(tutorialText.color.r, tutorialText.color.b, tutorialText.color.g, t);
            yield return null;
        }
        tutorialPanel.SetActive(false);
    }

    public void Resume() => PauseGame(false);
    public void Return() => GlobalData.instance.LoadScene("MainMenu");
}
