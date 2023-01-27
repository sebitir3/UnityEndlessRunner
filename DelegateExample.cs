using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateExample : MonoBehaviour
{
    delegate void ExampleDelegate();

    delegate float DoMath(float a, float b);

    //Create two methods called add and subtract
    //take in two float parameters
    //return back the addition or subtract of those two parameters

    float add(float a, float b)
    {
        return a + b;
    }

    float subtract(float x, float y)
    {
        return x - y;
    }

    void Start()
    {
        DoMath doMathVariable = add;

        Debug.Log(doMathVariable(6, 4)); //result 6 + 4 = 10

        doMathVariable += subtract; //a delegate can only store one method at a time

        Debug.Log(doMathVariable(6, 4)); //result 6 - 4 = 2

        //store a lambda exoression into a delegate
        doMathVariable = (a, b) => a * b;

        Debug.Log(doMathVariable(6, 4)); //result: 24
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
