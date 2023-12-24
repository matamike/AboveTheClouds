using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RotationConstraintUtility{
    public static Vector3 GetConstainedRotation(float minXAxisClampAngle, float maxXAxisClampAngle, float minYAxisClampAngle, float maxYAxisClampAngle, float minZAxisClampAngle, float maxZAxisClampAngle, Transform testTarget){
        if (testTarget == null) return default(Vector3);
 
        //-> Note: Range (-1, 1) : (<0)-> [-180, 0], (>0) -> [0, 180]
        float xAxisRotationOrientation = Mathf.Sin(testTarget.transform.localEulerAngles.x * Mathf.Deg2Rad);
        float yAxisRotationOrientation = Mathf.Sin(testTarget.transform.localEulerAngles.y * Mathf.Deg2Rad);
        float zAxisRotationOrientation = Mathf.Sin(testTarget.transform.localEulerAngles.z * Mathf.Deg2Rad);
        //Holders
        float newXRotation = testTarget.transform.localEulerAngles.x;
        float newYRotation = testTarget.transform.localEulerAngles.y;
        float newZRotation = testTarget.transform.localEulerAngles.z;

        //X Axis Clamp Calculation
        if (xAxisRotationOrientation > 0) newXRotation = Mathf.Clamp(testTarget.transform.localEulerAngles.x, minXAxisClampAngle, maxXAxisClampAngle);
        else if (xAxisRotationOrientation < 0) newXRotation = Mathf.Clamp(testTarget.transform.localEulerAngles.x, 360f + minXAxisClampAngle, 360f + maxXAxisClampAngle);

        //Y Axis Clamp Calculation
        if (yAxisRotationOrientation > 0) newYRotation = Mathf.Clamp(testTarget.transform.localEulerAngles.y, minYAxisClampAngle, maxYAxisClampAngle);
        else if (yAxisRotationOrientation < 0) newYRotation = Mathf.Clamp(testTarget.transform.localEulerAngles.y, 360f + minYAxisClampAngle, 360f + maxYAxisClampAngle);

        //Z Axis Clamp Calculation
        if (zAxisRotationOrientation > 0) newZRotation = Mathf.Clamp(testTarget.transform.localEulerAngles.z, minZAxisClampAngle, maxZAxisClampAngle);
        else if (zAxisRotationOrientation < 0) newZRotation = Mathf.Clamp(testTarget.transform.localEulerAngles.z, 360f + minZAxisClampAngle, 360f + maxZAxisClampAngle);

        //Final Rotation After Clamping to Limits
        testTarget.transform.localEulerAngles = new Vector3(newXRotation, newYRotation, newZRotation);

        return testTarget.transform.localEulerAngles;
    }


    public static void CalibrateStartingLockLimits(ref float minXClamp, ref float maxXClamp, ref float minYClamp, ref float maxYClamp, ref float minZClamp, ref float maxZClamp,Transform testTarget){
        //X Axis
        if (testTarget.transform.localEulerAngles.x > 0f && testTarget.transform.localEulerAngles.x <= 180f)
        {
            minXClamp += testTarget.transform.localEulerAngles.x;
            maxXClamp += testTarget.transform.localEulerAngles.x;
            if (minXClamp > 180f) minXClamp = (180f - (minXClamp - 180f)) * -1f;
            if (maxXClamp > 180f) maxXClamp = (180f - (maxXClamp - 180f)) * -1f;
        }
        else if (testTarget.transform.localEulerAngles.x > 180f)
        {
            minXClamp -= (360f - testTarget.transform.localEulerAngles.x);
            maxXClamp -= (360f - testTarget.transform.localEulerAngles.x);
            if (Mathf.Abs(minXClamp) > 180f) minXClamp = (180f - (minXClamp - 180f)) * -1f;
            if (Mathf.Abs(maxXClamp) > 180f) maxXClamp = (180f - (maxXClamp - 180f)) * -1f;
        }


        //Y Axis
        if (testTarget.transform.localEulerAngles.y > 0f && testTarget.transform.localEulerAngles.y <= 180f)
        {
            minYClamp += testTarget.transform.localEulerAngles.y;
            maxYClamp += testTarget.transform.localEulerAngles.y;
            if (minYClamp > 180f) minYClamp = (180f - (minYClamp - 180f)) * -1f;
            if (maxYClamp > 180f) maxYClamp = (180f - (maxYClamp - 180f)) * -1f;
        }
        else if (testTarget.transform.localEulerAngles.y > 180f)
        {
            minYClamp -= (360f - testTarget.transform.localEulerAngles.y);
            maxYClamp -= (360f - testTarget.transform.localEulerAngles.y);
            if (Mathf.Abs(minYClamp) > 180f) minYClamp = (180f - (minYClamp - 180f)) * -1f;
            if (Mathf.Abs(minYClamp) > 180f) maxYClamp = (180f - (maxYClamp - 180f)) * -1f;
        }

        //Z Axis
        if (testTarget.transform.localEulerAngles.z > 0f && testTarget.transform.localEulerAngles.z <= 180f)
        {
            minZClamp += testTarget.transform.localEulerAngles.x;
            maxZClamp += testTarget.transform.localEulerAngles.x;
            if (minZClamp > 180f) minZClamp = (180f - (minZClamp - 180f)) * -1f;
            if (maxZClamp > 180f) maxZClamp = (180f - (maxZClamp - 180f)) * -1f;
        }
        else if (testTarget.transform.localEulerAngles.z > 180f)
        {
            minZClamp -= (360f - testTarget.transform.localEulerAngles.z);
            maxZClamp -= (360f - testTarget.transform.localEulerAngles.z);
            if (Mathf.Abs(minZClamp) > 180f) minZClamp = (180f - (minZClamp - 180f)) * -1f;
            if (Mathf.Abs(maxZClamp) > 180f) maxZClamp = (180f - (maxZClamp - 180f)) * -1f;
        }
    }
}
