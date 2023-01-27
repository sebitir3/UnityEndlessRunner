using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFirstScript : MonoBehaviour
{
    //Value types / primitive types - direct acess to data
    // integers, floats, doubles, booleans, character 'a', struct

    //reference types - store references to data
    // strings, class, objects, interfaces, delegates

    

    // Awake is called before 1st frame update, before start

    private void Awake()
    {
        Debug.Log("Awake");
    }

    // Start is called before the first frame update, after awake

    void Start()
    {
        Debug.Log("Start");
    }

    //OnEnable - called every time a GameObject is active within the scene

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    //FixedUpdate - is called once per frame at fixed time proportion
    // -- used for physics

    private void FixedUpdate()
    {
        Debug.Log("FixedUpdate");
    }

    //LateUpdate - called once per frame - usedful for when everything else has calculated 
    // -- useful for camera position

    private void LateUpdate()
    {
        Debug.Log("LateUpdate");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update");

    }
}
