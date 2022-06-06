using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsTrigger : MonoBehaviour
{
    [SerializeField] float time = 10f;
    void Start()
    {
        StartCoroutine("Countdown");
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(time);
        GlobalData.instance.LoadScene("Credits");
    }
}
