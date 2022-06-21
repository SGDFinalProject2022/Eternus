using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LockerDoor : MonoBehaviour
{
    [SerializeField] string lockerText;
    [SerializeField] Text code;
    [SerializeField] int codeLength = 4;
    [SerializeField] Text codePaperText;
    [SerializeField] List<GameObject> panels = new List<GameObject>();
    [SerializeField] UI ui;
    public UnityEvent onUnlock;

    string finalCode;

    bool panelIsOpen;
    PlayerMovement mov;

    void Start()
    {
        foreach(GameObject obj in panels)
        {
            obj.SetActive(false);
        }
        panelIsOpen = false;
        for(int i = 0; i < codeLength; i++)
        {
            int randomNumber = Random.Range(0, 10);
            finalCode += randomNumber.ToString();
        }
        codePaperText.text = lockerText + " " + finalCode; 
    }

    void Update()
    {
        if(Input.GetKeyDown("escape"))
        {
            ClosePanel();
        }
    }

    public void ClosePanel()
    {
        if (panelIsOpen)
        {
            ui.headBobController.enableHeadbob = true;
            Cursor.lockState = CursorLockMode.Locked;
            panelIsOpen = false;
            ui.move.enabled = true;
            ui.look.enabled = true; 
            foreach (GameObject obj in panels)
            {
                obj.SetActive(false);
            }
            code.text = "";
            StartCoroutine("AllowPause");
        }
    }

    IEnumerator AllowPause()
    {
        yield return new WaitForSeconds(.025f);
        ui.panelIsOpen = false;
    }

    public void OpenPanel(GameObject panel)
    {
        if (!panelIsOpen)
        {
            ui.headBobController.enableHeadbob = false;
            Cursor.lockState = CursorLockMode.None;
            ui.panelIsOpen = true;
            ui.move.enabled = false;
            ui.look.enabled = false;
            panelIsOpen = true;
            panel.SetActive(true);
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
            ClosePanel();
            onUnlock.Invoke();
            this.gameObject.layer = 0;
        }
        else
        {
            yield return new WaitForSeconds(1f);
            code.text = "";
        }
    }
}
