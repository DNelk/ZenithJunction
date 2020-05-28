using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCam : MonoBehaviour
{
    public float Speed = 0.5f;
    public float rotationX, rotationY, sensitivityX, sensitivityY, maximumX, minimumX, maximumY, minimumY;

    private void Start()
    {
        rotationX = 0;
        rotationY = 0;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.T))
            transform.position += transform.forward * Speed;
        if(Input.GetKey(KeyCode.F))
            transform.position += transform.right * -1 * Speed;
        if(Input.GetKey(KeyCode.G))
            transform.position += transform.forward * -1 * Speed;
        if(Input.GetKey(KeyCode.H))
            transform.position += transform.right * Speed;
        
        ///Read Mouse input
        rotationX += Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivityY* Time.deltaTime;
        transform.eulerAngles = new Vector3(rotationY, rotationX, 0);
    }
    
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

}
