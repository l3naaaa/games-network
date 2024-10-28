using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    //public float smoothness = 5.0f;
    public Vector3 cameraOffset;
    
    
    void Start()
    {
        cameraOffset = transform.position - playerTransform.position;
    }

    void Update()
    {
        if(playerTransform == null)
        {
        return;
        }

       transform.position = playerTransform.position + cameraOffset;

        //transform.LookAt(target);

    }
}
