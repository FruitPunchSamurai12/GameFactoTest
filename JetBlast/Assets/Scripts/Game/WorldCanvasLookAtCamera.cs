using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvasLookAtCamera : MonoBehaviour
{

    Camera cam;
    private void Awake()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(cam.transform);
    }
}
