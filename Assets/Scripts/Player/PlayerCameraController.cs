using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] float camSenseX, camSenseY;
    [SerializeField] float minXRot, maxXRot;

    Vector2 cameraRotation;
    Transform camTransform;

    private void Start()
    {
        camTransform = Camera.main.transform;
     
        cameraRotation = new(camTransform.eulerAngles.x, transform.eulerAngles.y);
  
    }
    void Update()
    {
        if (GameState.GamePaused)
            return;
       
        cameraRotation.y += Input.GetAxisRaw("Mouse X") * camSenseX * GlobalSettings.mouseSensitivty;
      
        cameraRotation.x -= Input.GetAxisRaw("Mouse Y") * camSenseY * GlobalSettings.mouseSensitivty;

      
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, minXRot, maxXRot);

       
        camTransform.localEulerAngles = new Vector3(cameraRotation.x, 0f, 0f);
      
        transform.eulerAngles = new Vector3(0f, cameraRotation.y, 0f);

        DevText.DisplayInfo("cam", "Camera Rotation " + cameraRotation, "Camera");


    }
}
