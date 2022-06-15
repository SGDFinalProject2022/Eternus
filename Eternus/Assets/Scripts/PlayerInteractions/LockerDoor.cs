using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LockerDoor : MonoBehaviour
{
    [SerializeField] GameObject codePanel;
    [SerializeField] Text code;
    [SerializeField] int codeLength = 4;
    [SerializeField] Text codePaperText;
    [SerializeField] GameObject codePaper;
    public UnityEvent onUnlock;

    string finalCode;

    bool panelIsOpen;
    PlayerMovement mov;
    UI ui;

    void Start()
    {
        codePaper.SetActive(false);
        codePanel.SetActive(false);
        panelIsOpen = false;
        for(int i = 0; i < codeLength; i++)
        {
            int randomNumber = Random.Range(0, 10);
            finalCode += randomNumber.ToString();
        }
        print(finalCode);
        codePaperText.text = "Storage Locker Code: " + finalCode; 
    }

    void Update()
    {
        if(Input.GetKeyDown("escape"))
        {
            ClosePanel();
        }
    }
    
    public void OpenCodePanel(UI uiRef)
    {        
        if(ui == null)
        {
            ui = uiRef;
        }
        if(!panelIsOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            ui.panelIsOpen = true;
            ui.move.enabled = false;
            ui.look.enabled = false;
            panelIsOpen = true;
            codePanel.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        if (panelIsOpen)
        {
            print("Running");
            Cursor.lockState = CursorLockMode.Locked;
            panelIsOpen = false;
            ui.move.enabled = true;
            ui.look.enabled = true;
            codePanel.SetActive(false);
            codePaper.SetActive(false);
            code.text = "";
            StartCoroutine("AllowPause");
        }
    }

    IEnumerator AllowPause()
    {
        yield return new WaitForSeconds(.025f);
        ui.panelIsOpen = false;
    }

    public void PaperOpen(UI uiRef)
    {
        if (ui == null)
        {
            ui = uiRef;
        }
        if (!panelIsOpen)
        {
            ui.panelIsOpen = true;
            ui.move.enabled = false;
            ui.look.enabled = false;
            panelIsOpen = true;
            codePaper.SetActive(true);
        }        
    }

    public void InputKey(string key)
    {
        if(code.text.Length < codeLength)
        {
            code.text += key;
        }
        
        if(code.text.Length == codeLength)
        {
            StartCoroutine("Submit");
        }
    }

    public void DeleteKey()
    {
        if(code.text.Length > 0)
        {
            code.text = code.text.Substring(0, code.text.Length - 1);
        }
    }

    IEnumerator Submit()
    {
        if(code.text == finalCode)
        {
            print("You got it!");
            ClosePanel();
            onUnlock.Invoke();
        }
        else
        {
            print("That was incorrect...");
            yield return new WaitForSeconds(1f);
            code.text = "";
        }
    }
}
