using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public float xDistance, yDistance, zDistance;
    public GameObject target;
    
    void Start()
    {
        target = GameObject.Find("Sphere");
    }

    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.E))
        {
            //rotate camera clockwise
            transform.Rotate(Vector3.up, Time.deltaTime * 180);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            //rotate camera counterclockwise
            transform.Rotate(Vector3.up, Time.deltaTime * -180);
        }

        // moving camera on the target then back the distance we set
        transform.position = target.transform.position + transform.forward * zDistance;
        transform.Translate(Vector3.up * yDistance);



    }
}
