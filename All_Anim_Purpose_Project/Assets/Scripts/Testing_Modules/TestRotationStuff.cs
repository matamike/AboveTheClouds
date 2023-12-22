using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestRotationStuff : MonoBehaviour{
    [SerializeField] private Transform testTarget;
    [SerializeField] private Transform lookAtTarget;
    [SerializeField][Range(-179.9f, 179.9f)] private float minYAxisClampAngle;
    [SerializeField][Range(-179.9f, 179.9f)] private float maxYAxisClampAngle;
    [SerializeField][Range(-179.9f, 179.9f)] private float minXAxisClampAngle;
    [SerializeField][Range(-179.9f, 179.9f)] private float maxXAxisClampAngle;
    [SerializeField][Range(-179.9f, 179.9f)] private float minZAxisClampAngle;
    [SerializeField][Range(-179.9f, 179.9f)] private float maxZAxisClampAngle;

    private void Start(){
        CalibrateStartingLockLimits();
    }

    void Update(){
        testTarget.LookAt(lookAtTarget, Vector3.up);
        LockToTarget();
    }

    private void LockToTarget(){
        //-> Note: Range (-1, 1) : (<0)-> [-180, 0], (>0) -> [0, 180]
        float xAxisRotationOrientation = Mathf.Sin(testTarget.transform.localEulerAngles.x * Mathf.Deg2Rad);
        float yAxisRotationOrientation = Mathf.Sin(testTarget.transform.localEulerAngles.y * Mathf.Deg2Rad); 
        float zAxisRotationOrientation = Mathf.Sin(testTarget.transform.localEulerAngles.z * Mathf.Deg2Rad);
        //Holders
        float newXRotation = testTarget.transform.localEulerAngles.x;
        float newYRotation = testTarget.transform.localEulerAngles.y;
        float newZRotation = testTarget.transform.localEulerAngles.z;

        //X Axis Clamp Calculation
        if (xAxisRotationOrientation > 0) newXRotation = Mathf.Clamp(testTarget.transform.eulerAngles.x, minXAxisClampAngle, maxXAxisClampAngle);
        else if(xAxisRotationOrientation < 0) newXRotation = Mathf.Clamp(testTarget.transform.eulerAngles.x, 360f + minXAxisClampAngle, 360f + maxXAxisClampAngle);

        //Y Axis Clamp Calculation
        if (yAxisRotationOrientation > 0) newYRotation = Mathf.Clamp(testTarget.transform.eulerAngles.y, minYAxisClampAngle, maxYAxisClampAngle);
        else if (yAxisRotationOrientation < 0) newYRotation = Mathf.Clamp(testTarget.transform.eulerAngles.y, 360f + minYAxisClampAngle, 360f + maxYAxisClampAngle);

        //Z Axis Clamp Calculation
        if (zAxisRotationOrientation > 0) newZRotation = Mathf.Clamp(testTarget.transform.eulerAngles.z, minZAxisClampAngle, maxZAxisClampAngle);
        else if (zAxisRotationOrientation < 0) newZRotation = Mathf.Clamp(testTarget.transform.eulerAngles.z, 360f + minZAxisClampAngle, 360f + maxZAxisClampAngle);

        //Final Rotation After Clamping to Limits
        testTarget.transform.eulerAngles = new Vector3(newXRotation, newYRotation, newZRotation);
    }

    private void CalibrateStartingLockLimits(){
        //X Axis
        if (testTarget.transform.eulerAngles.x > 0f && testTarget.transform.eulerAngles.x <= 180f)
        {
            minXAxisClampAngle += testTarget.transform.eulerAngles.x;
            maxXAxisClampAngle += testTarget.transform.eulerAngles.x;
            if (minXAxisClampAngle > 180f) minXAxisClampAngle = (180f - (minXAxisClampAngle - 180f)) * -1f;
            if (maxXAxisClampAngle > 180f) maxXAxisClampAngle = (180f - (maxXAxisClampAngle - 180f)) * -1f;
        }
        else if (testTarget.transform.eulerAngles.x > 180f)
        {
            minXAxisClampAngle -= (360f - testTarget.transform.eulerAngles.x);
            maxXAxisClampAngle -= (360f - testTarget.transform.eulerAngles.x);
            if (Mathf.Abs(minXAxisClampAngle) > 180f) minXAxisClampAngle = (180f - (minXAxisClampAngle - 180f)) * -1f;
            if (Mathf.Abs(maxXAxisClampAngle) > 180f) maxXAxisClampAngle = (180f - (maxXAxisClampAngle - 180f)) * -1f;
        }


        //Y Axis
        if (testTarget.transform.eulerAngles.y > 0f && testTarget.transform.eulerAngles.y <= 180f){
            minYAxisClampAngle += testTarget.transform.eulerAngles.y;
            maxYAxisClampAngle += testTarget.transform.eulerAngles.y;
            if (minYAxisClampAngle > 180f) minYAxisClampAngle = (180f - (minYAxisClampAngle - 180f)) * -1f;
            if (maxYAxisClampAngle > 180f) maxYAxisClampAngle = (180f - (maxYAxisClampAngle - 180f)) * -1f;
        }
        else if(testTarget.transform.eulerAngles.y > 180f)
        {
            minYAxisClampAngle -= (360f - testTarget.transform.eulerAngles.y);
            maxYAxisClampAngle -= (360f - testTarget.transform.eulerAngles.y);
            if (Mathf.Abs(minYAxisClampAngle) > 180f) minYAxisClampAngle = (180f - (minYAxisClampAngle - 180f)) * -1f;
            if (Mathf.Abs(minYAxisClampAngle) > 180f) maxYAxisClampAngle = (180f - (maxYAxisClampAngle - 180f)) * -1f;
        }

        //Z Axis
        if (testTarget.transform.eulerAngles.z > 0f && testTarget.transform.eulerAngles.z <= 180f)
        {
            minZAxisClampAngle += testTarget.transform.eulerAngles.x;
            maxZAxisClampAngle += testTarget.transform.eulerAngles.x;
            if (minZAxisClampAngle > 180f) minZAxisClampAngle = (180f - (minZAxisClampAngle - 180f)) * -1f;
            if (maxZAxisClampAngle > 180f) maxZAxisClampAngle = (180f - (maxZAxisClampAngle - 180f)) * -1f;
        }
        else if (testTarget.transform.eulerAngles.z > 180f)
        {
            minZAxisClampAngle -= (360f - testTarget.transform.eulerAngles.z);
            maxZAxisClampAngle -= (360f - testTarget.transform.eulerAngles.z);
            if (Mathf.Abs(minZAxisClampAngle) > 180f) minZAxisClampAngle = (180f - (minZAxisClampAngle - 180f)) * -1f;
            if (Mathf.Abs(maxZAxisClampAngle) > 180f) maxZAxisClampAngle = (180f - (maxZAxisClampAngle - 180f)) * -1f;
        }
    }
}