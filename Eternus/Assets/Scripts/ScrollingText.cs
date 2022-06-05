using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingText : MonoBehaviour
{
    [SerializeField] GameObject textBox;
    [SerializeField] float scrollSpeed = 3f;
    [SerializeField] float creditsTimer = 20f;

    void Start()
    {
        textBox.transform.position = new Vector2(Screen.width/2, -75);
        StartCoroutine("Countdown");
    }

    void Update()
    {
        textBox.transform.Translate(Vector3.up * Time.deltaTime * scrollSpeed, Space.World);
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(creditsTimer);
        GlobalData.instance.LoadScene("MainMenu");
    }
}
