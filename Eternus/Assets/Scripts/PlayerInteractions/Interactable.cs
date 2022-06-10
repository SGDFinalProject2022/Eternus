using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Attached to interactable objects
/// </summary>
public class Interactable : MonoBehaviour
{
    public UnityEvent onInteract;
    public int ID;
    //public string interactText;

    // Start is called before the first frame update
    void Start()
    {
        ID = Random.Range(0, 9999);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DebugPrint(string message)
    {
        Debug.Log(message);
    }   
    public void ChangeScene(string sceneName)
    {
        if(GlobalData.instance != null)
        {
            GlobalData.instance.LoadScene(sceneName);
        }       
    }
}
