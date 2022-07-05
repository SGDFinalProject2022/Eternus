using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoFade : MonoBehaviour
{
    [SerializeField] float timer = 5f;
    void Start()
    {
        StartCoroutine("Countdown");
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(timer);
        GlobalData.instance.LoadScene("MainMenu");
    }
}
