using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingText : MonoBehaviour
{
    [SerializeField] GameObject textBox;
    [SerializeField] float scrollSpeed = 3f;

    void Update()
    {
        textBox.transform.Translate(Vector3.up * Time.deltaTime * scrollSpeed, Space.World);
    }
}
