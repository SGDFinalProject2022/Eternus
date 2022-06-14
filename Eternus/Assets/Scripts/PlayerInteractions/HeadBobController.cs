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
    [Range(0, 30f)] public float frequency = 10f;

    [SerializeField] Transform playerCamera;
    [SerializeField] Transform cameraHolder;

    Vector3 startPos;
    PlayerMovement playerMovement;

    private void Awake()
    {
        startPos = playerCamera.localPosition;
        playerMovement = GetComponent<PlayerMovement>();
    }

    private Vector3 GetFootStepMotion()
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
        if (playerMovement.speed > 1)
        { playerCamera.localPosition += GetFootStepMotion(); }
        
        ResetPosition();
        playerCamera.LookAt(FocusTarget());
    }
}
