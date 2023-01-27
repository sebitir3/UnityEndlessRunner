using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    public float speed = 20f;
    public float jump = 100f;

    public bool onGround = false;
    private Rigidbody rbody;

    private Transform tCamera;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        tCamera = GameObject.Find("Main Camera").transform;
    }

    void FixedUpdate()
    {
        Vector3 cForeward = tCamera.forward;
        Vector3 cRight = tCamera.right;

        float boost = 1f;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            boost = 2;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rbody.AddForce(Vector3.up * jump * boost);
        }


        if (Input.GetKey(KeyCode.W))
        {
            //move forward
            rbody.AddForce(cForeward * speed * boost);
        }

        if (Input.GetKey(KeyCode.S))
        {
            //move backwards
            rbody.AddForce(-cForeward * speed * boost);
        }

        if (Input.GetKey(KeyCode.A))
        {
            //move left
            rbody.AddForce(-cRight * speed * boost);
        }

        if (Input.GetKey(KeyCode.D))
        {
            //move right
            rbody.AddForce(cRight * speed * boost);
        }

        
    }
}
