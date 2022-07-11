using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NoteInteract : MonoBehaviour
{
    [SerializeField] List<GameObject> panels = new List<GameObject>();
    [SerializeField] UI ui;
    [SerializeField] AudioManager audioMan;
    bool panelIsOpen;
    PlayerMovement player;
    bool isHovering;

    void Start()
    {
        player = ui.transform.parent.gameObject.GetComponent<PlayerMovement>();
        foreach (GameObject obj in panels)
        {
            obj.SetActive(false);
        }
        panelIsOpen = false;
        isHovering = false;
    }

    void Update()
    {
        if(panelIsOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClosePanel();
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (!isHovering)
                {
                    ClosePanel();
                }
            }
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
        if (!panelIsOpen && !player.isCrouching)
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

    public void UpdateHover(bool isHover)
    {
        print("Hover is " + isHover);
        isHovering = isHover;
    }
}
