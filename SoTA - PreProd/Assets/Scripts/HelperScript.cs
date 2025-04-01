using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperScript
{
    public static Vector3 RotateVector3(Vector3 vectorToRotate, float degrees, Vector3 rotationAxis)
    {
        Vector3 rotatedVector = Quaternion.AngleAxis(degrees, rotationAxis) * vectorToRotate;
        return rotatedVector;
    }
}
