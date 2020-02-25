using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating_Self : MonoBehaviour
{
    public rotateDirection direction;
    private float direction_Multiply;
    public float speed;


    // Start is called before the first frame update
    void Start()
    {
        
        //set if the gear will rotate clockwise or anti-clockwise
        if (direction == rotateDirection.Clockwise)
            direction_Multiply = -1;
        else
        {
            {
                direction_Multiply = 1;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.forward * speed * direction_Multiply * Time.deltaTime);
    }

    public enum rotateDirection
    {
        Clockwise,
        Anti_Clockwise
    }
}
