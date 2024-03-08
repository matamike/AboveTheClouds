using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BodypartsController : MonoBehaviour{
    [SerializeField] Transform _bodyPartTransform;
    [SerializeField] bool isHandled = false; //default false
    [SerializeField][Range(-179.9f, 179.9f)] private float minYAxisClampAngle;
    [SerializeField][Range(-179.9f, 179.9f)] private float maxYAxisClampAngle;
    [SerializeField][Range(-179.9f, 179.9f)] private float minXAxisClampAngle;
    [SerializeField][Range(-179.9f, 179.9f)] private float maxXAxisClampAngle;
    [SerializeField][Range(-179.9f, 179.9f)] private float minZAxisClampAngle;
    [SerializeField][Range(-179.9f, 179.9f)] private float maxZAxisClampAngle;
    private Vector3 lookAtForwardDirection, lookAtUp;
    private Vector3 lastKnownDirection;
    private Vector3 newDirection;
    private bool directionChanged = false;

    private void Start(){
        lastKnownDirection = transform.forward;
        newDirection = transform.forward;
        lookAtUp = transform.up;
        RotationConstraintUtility.CalibrateStartingLockLimits(ref minXAxisClampAngle, ref maxXAxisClampAngle, ref minYAxisClampAngle, ref maxYAxisClampAngle, ref minZAxisClampAngle, ref maxZAxisClampAngle, gameObject.transform);
    }

    private void LateUpdate(){
        if (directionChanged){
            float speed = 2f;
            lastKnownDirection = Vector3.Lerp(lastKnownDirection, newDirection, speed);
            transform.LookAt(transform.position  + lastKnownDirection, lookAtUp);
            Vector3 newEuler = GetClampedPartEuler();    

            float lerpXAxis, lerpYAxis, lerpZAxis;
            if (newEuler.x > 180f && transform.localEulerAngles.x < 180f) {
                lerpXAxis = Mathf.Lerp(transform.eulerAngles.x, newEuler.x - 360f, speed);
            }
            else lerpXAxis = Mathf.Lerp(transform.eulerAngles.x, newEuler.x, speed);

            if (newEuler.y > 180f && transform.localEulerAngles.y < 180f){
                lerpYAxis = Mathf.Lerp(transform.eulerAngles.y, newEuler.y - 360f, speed);
            }
            else lerpYAxis = Mathf.Lerp(transform.eulerAngles.y, newEuler.y, speed);

            if (newEuler.z > 180f && transform.localEulerAngles.z < 180f){
                lerpZAxis = Mathf.Lerp(transform.eulerAngles.z, newEuler.z - 360f, speed);
            }
            else lerpZAxis = Mathf.Lerp(transform.eulerAngles.z, newEuler.z, speed);

            transform.localEulerAngles = new Vector3(lerpXAxis, lerpYAxis, lerpZAxis);
        }
    }

    private void OnEnable(){
        CameraController.OnDirectionChanged += CameraFollow_OnDirectionChanged;
    }

    private void OnDisable(){
        CameraController.OnDirectionChanged -= CameraFollow_OnDirectionChanged;
    }

    private void CameraFollow_OnDirectionChanged(object sender, EventArgs e) => FaceDirection(_bodyPartTransform);

    private void FaceDirection(Transform bodypart){
        if (isHandled){
            lookAtForwardDirection = CameraController.Instance.GetCameraForward();
            lookAtUp = transform.up;

            float dotProductPlayerCamera = Mathf.Round(Vector3.Dot(lookAtForwardDirection, transform.parent.forward));

            if (PlayerController.Instance.HasMovingDirection()) {
                directionChanged = false;
                bodypart.LookAt(transform.position + transform.parent.forward, transform.parent.up);
            }
            else {
                directionChanged = true;
                if (dotProductPlayerCamera == 1) newDirection = CameraController.Instance.GetCameraForward();
                else if (dotProductPlayerCamera == -1) newDirection = CameraController.Instance.GetCameraBack();
            }
        }
    }

    private Vector3 GetClampedPartEuler(){
        return RotationConstraintUtility.GetConstainedRotation(minXAxisClampAngle, maxXAxisClampAngle, minYAxisClampAngle, maxYAxisClampAngle, minZAxisClampAngle, maxZAxisClampAngle, gameObject.transform);
    }
}