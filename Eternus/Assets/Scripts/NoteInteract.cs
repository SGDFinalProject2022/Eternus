using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NoteInteract : MonoBehaviour
{
    [SerializeField] List<GameObject> panels = new List<GameObject>();
    [SerializeField] UI ui;
    [SerializeField] AudioManager audioMan;
    public UnityEvent onUnlock;
    bool panelIsOpen;
    PlayerMovement mov;

    void Start()
    {
        foreach (GameObject obj in panels)
        {
            obj.SetActive(false);
        }
        panelIsOpen = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
            StartCoroutine("AllowPause");
        }
    }

    IEnumerator AllowPause()
    {
        yield return new WaitForSeconds(.025f);
        if (audioMan != null) { audioMan.Play("Note Drop"); }
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
            if (audioMan != null) { audioMan.Play("Note Pickup"); }
        }
    }
}
