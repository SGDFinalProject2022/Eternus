using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls the camera headbob movement, modified by PlayerMovement
/// </summary>
public class HeadBobController : MonoBehaviour
{
    public bool enableHeadbob = true;
    public float amplitude = 0.0005f;
    [SerializeField, Range(0, 30f)] public float frequency = 10f;

    [SerializeField] Transform playerCamera;
    [SerializeField] Transform cameraHolder;

    Vector3 startPos;
    CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        startPos = playerCamera.localPosition;
    }

    void PlayMotion(Vector3 motion)
    {
        playerCamera.localPosition += motion;
    }

    Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y = Mathf.Sin(Time.time * frequency) * amplitude;
        pos.x = Mathf.Cos(Time.time * frequency) * amplitude * 2;
        return pos;
    }

    void ResetPosition()
    {
        if (playerCamera.localPosition == startPos) return;
        playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, startPos, 1 * Time.deltaTime);
    }

    Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + cameraHolder.localPosition.y, transform.position.z);
        pos += cameraHolder.forward * 15.0f;
        return pos;
    }

    // Update is called once per frame
    void Update()
    {
        if (!enableHeadbob) { return; }
        PlayMotion(FootStepMotion());
        ResetPosition();
        playerCamera.LookAt(FocusTarget());
    }
}
