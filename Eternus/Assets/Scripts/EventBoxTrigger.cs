using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Attached to box trigger to invoke an event when the player enters the trigger
/// </summary>
public class EventBoxTrigger : MonoBehaviour
{
    public UnityEvent onTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            onTrigger.Invoke();
        }
    }

    public void DebugLog(string text)
    {
        Debug.Log(text);
    }
}
