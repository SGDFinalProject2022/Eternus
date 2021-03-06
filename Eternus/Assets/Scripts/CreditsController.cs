using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
    [SerializeField] GameObject skipPanel;
    bool panelOpen;

    void Awake()
    {
        skipPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if(Input.GetKeyDown("escape"))
        {
            if(panelOpen)
            {
                skipPanel.SetActive(false);
                panelOpen = false;
            }
            else
            {
                skipPanel.SetActive(true);
                panelOpen = true;
            }
        }
    }

    public void OnSkip()
    {
        GetComponent<AudioManager>().VolumeFadeOut("Title", true);
        skipPanel.SetActive(false);
        panelOpen = false;
        GlobalData.instance.LoadScene("MainMenu");
    }

    public void OnCancel()
    {
        skipPanel.SetActive(false);
        panelOpen = false;
    }
}
