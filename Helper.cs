using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper 
{
    //modify the x position of a transform
    public static void SetX(Transform t, float x)
    {
        t.position = new Vector3(x, t.position.y, t.position.z);
    }

    //modify the y position of a transform
    public static void SetY(Transform t, float y)
    {
        t.position = new Vector3(t.position.x, y, t.position.z);
    }

    //modify the z position of a transform
    public static void SetZ(Transform t, float z)
    {
        t.position = new Vector3(t.position.x, t.position.y, z);
    }
}
