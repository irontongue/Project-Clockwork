using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] float cameraSensitivity;
    [SerializeField] float minXRot, maxXRot;

    Vector2 cameraRotation;
    Transform camTransform;

    private void Start()
    {
        camTransform = Camera.main.transform;
    }
    void Update()
    {

        cameraRotation.y += Input.GetAxisRaw("Mouse X") * cameraSensitivity;
        cameraRotation.x -= Input.GetAxisRaw("Mouse Y") * cameraSensitivity;

        cameraRotation.x = Mathf.Clamp(cameraRotation.x, minXRot, maxXRot);

        camTransform.eulerAngles = cameraRotation;

        DevText.DisplayInfo("cam", "Camera Rotation " + cameraRotation, "Camera");


    }
}
