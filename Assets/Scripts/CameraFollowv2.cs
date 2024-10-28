using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollowv2 : MonoBehaviour
{
    public Transform target;
    public Vector2 offset;
    public float yMinLimit, yMaxLimit;
    public float xSpeed, ySpeed;
    public float distance = 3;

    public float x, y;

    public static CameraFollowv2 instance;
    private void Awake()
    {
        instance = this;

        if (this.enabled)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void LateUpdate()
    {
        if(target == null) { return; }
        if (distance < 2) distance = 2;
       // distance -= Input.GetAxis("Mouse ScrollWheel") * 2;
        
        var pos = Input.mousePosition;
        var dpiScale = 1f;
        if (Screen.dpi < 1) dpiScale = 1;
        if (Screen.dpi < 200) dpiScale = 1;
        else dpiScale = Screen.dpi / 200f;
        

        x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

        y = ClampAngle(y, yMinLimit, yMaxLimit);
        var rotation = Quaternion.Euler(y, x, 0);
        var position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.transform.position;

        position += transform.right * offset.x;
        position += transform.forward * offset.y;

        transform.rotation = rotation;
        transform.position = position;


        if (Mathf.Abs(prevDistance - distance) > 0.001f)
        {
            prevDistance = distance;
            var rot = Quaternion.Euler(y, x, 0);
            var po = rot * new Vector3(0.0f, 0.0f, -distance) + target.transform.position;
            transform.rotation = rot;
            transform.position = po;
        }
    }
    float prevDistance;


    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}

