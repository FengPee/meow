using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    public Camera cameraToLookAt; // using our main camera to follow

    void Update()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position; // finds the position of the main camera
        v.x = v.z = 0.0f;
        transform.LookAt(cameraToLookAt.transform.position - v); // makes the health bar face the camera
        transform.Rotate(0, 180, 0);
    }
}
