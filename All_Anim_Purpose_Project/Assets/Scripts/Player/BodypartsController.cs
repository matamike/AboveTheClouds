using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodypartsController : MonoBehaviour{
    [SerializeField] Transform _bodyPartTransform;
    [SerializeField] bool isHandled = false; //default false
    private Vector3 lastKnownDirection;
    [SerializeField] private Vector2 xAxisDeadZone;
    [SerializeField] private Vector2 yAxisDeadZone;

    private void OnEnable(){
        CameraController.OnDirectionChanged += CameraFollow_OnDirectionChanged;
    }

    private void OnDisable(){
        CameraController.OnDirectionChanged -= CameraFollow_OnDirectionChanged;
    }

    private void CameraFollow_OnDirectionChanged(object sender, EventArgs e) => FaceDirection(_bodyPartTransform);

    private void FaceDirection(Transform bodypart){
        if (isHandled){
            Vector3 lookAtForwardDirection = CameraController.Instance.GetCameraForward();
            Vector3 lookAtUp = transform.up; 

            float dotProductPlayerCamera = Mathf.Round(Vector3.Dot(lookAtForwardDirection, transform.parent.forward));

            if (PlayerController.Instance.HasMovingDirection()) bodypart.LookAt(transform.position + transform.parent.forward, transform.parent.up);
            else {
                if (dotProductPlayerCamera == 1) lastKnownDirection = CameraController.Instance.GetCameraForward();
                else if (dotProductPlayerCamera == -1) lastKnownDirection = CameraController.Instance.GetCameraBack();

                //bodypart.rotation = Quaternion.LookRotation(lastKnownDirection, lookAtUp);
                bodypart.LookAt(transform.position + lastKnownDirection, lookAtUp);

                //Rotation Deadzone Check
                //float newRotXEuler =0f, newRotYEuler=0f,newRotZEuler = 0f;
                //X Axis               
                //if (bodypart.localEulerAngles.x < Mathf.Abs(xAxisDeadZone.x - 360f) && bodypart.localEulerAngles.x > xAxisDeadZone.y)
                //{
                //    Debug.Log("Clamp X Axis Rotation!");
                //    //Snap to nearest limit
                //    float upperLimit = Mathf.Abs(bodypart.localEulerAngles.x - Mathf.Abs(xAxisDeadZone.x - 360f));
                //    Debug.Log("UpperLimit: " + upperLimit);
                //    float lowerLimit = Mathf.Abs(bodypart.localEulerAngles.x - xAxisDeadZone.y);
                //    Debug.Log("LowerLimit: " + lowerLimit);
                //    if (upperLimit > lowerLimit){
                //        newRotXEuler = Mathf.Abs(xAxisDeadZone.x - 360f);
                //    }
                //    else{ 
                //        newRotXEuler = xAxisDeadZone.y;
                //    }
                //}

                //Y Axis
                //if (bodypart.localEulerAngles.y < Mathf.Abs(yAxisDeadZone.x - 360f) && bodypart.localEulerAngles.y > yAxisDeadZone.y)
                //{
                //    //Snap to nearest limit
                //    float upperLimit = Mathf.Abs(bodypart.localEulerAngles.y - Mathf.Abs(yAxisDeadZone.x - 360f));
                //    float lowerLimit = Mathf.Abs(bodypart.localEulerAngles.x - yAxisDeadZone.y);
                //    if (upperLimit > lowerLimit) newRotYEuler = Mathf.Abs(yAxisDeadZone.x - 360f);
                //    else newRotYEuler = yAxisDeadZone.y;
                //}

                ////Z Axis
                //if ((bodypart.localEulerAngles.z - zAxisMirroredLimit) > 0)
                //{
                //    newRotZEuler = zAxisMirroredLimit;
                //}
                //else if ((360f - zAxisMirroredLimit) < bodypart.localEulerAngles.z)
                //{
                //    newRotZEuler = 360f - zAxisMirroredLimit;
                //}
                //newRotZEuler = zAxisMirroredLimit; // default

                //if(newRotXEuler != 0f) {
                //bodypart.localEulerAngles = new Vector3(newRotXEuler, bodypart.localEulerAngles.y, bodypart.localEulerAngles.z);
                //}
                //if(newRotYEuler != 0f){
                //  bodypart.localEulerAngles = new Vector3(bodypart.localEulerAngles.x, newRotYEuler, bodypart.localEulerAngles.z);
                //}
                //if(newRotZEuler != 0f)
                //{
                //  bodypart.localEulerAngles = new Vector3(bodypart.localEulerAngles.x, bodypart.localEulerAngles.y, newRotZEuler);
                //}
                //Debug.Log(bodypart.name + " has rotation(Euler): " + bodypart.transform.eulerAngles);
                //Debug.Log(bodypart.name + " has rotation(Quaternion): " + bodypart.rotation.eulerAngles);
            }
        }
    }
}