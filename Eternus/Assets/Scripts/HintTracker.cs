using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintTracker : MonoBehaviour
{
    int hintCount = 0;
    [SerializeField] List<string> hint = new List<string>();
    [SerializeField] float timer = 10f;
    [SerializeField] UI ui;

    Coroutine countdown;

    void Start()
    {
        countdown = StartCoroutine("Countdown");
    }

    IEnumerator Countdown()
    {
        while(true)
        {
            yield return new WaitForSeconds(timer);
            ui.ShowObjective(hint[hintCount]);
        }
    }

    public void UpdateHint()
    {
        StopCoroutine(countdown);
        hintCount++;
        countdown = StartCoroutine("Countdown");
    }
}
