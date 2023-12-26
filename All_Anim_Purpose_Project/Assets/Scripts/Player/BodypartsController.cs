using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Start(){
        RotationConstraintUtility.CalibrateStartingLockLimits(ref minXAxisClampAngle, ref maxXAxisClampAngle, ref minYAxisClampAngle, ref maxYAxisClampAngle, ref minZAxisClampAngle, ref maxZAxisClampAngle, gameObject.transform);
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

            if (PlayerController.Instance.HasMovingDirection()) bodypart.LookAt(transform.position + transform.parent.forward, transform.parent.up);
            else {
                if (dotProductPlayerCamera == 1) lastKnownDirection = CameraController.Instance.GetCameraForward();
                else if (dotProductPlayerCamera == -1) lastKnownDirection = CameraController.Instance.GetCameraBack();
                Debug.Log("Stationary Face Direction Changed");
                bodypart.LookAt(transform.position + lastKnownDirection, lookAtUp);
                gameObject.transform.localEulerAngles = RotationConstraintUtility.GetConstainedRotation(minXAxisClampAngle, maxXAxisClampAngle, minYAxisClampAngle, maxYAxisClampAngle, minZAxisClampAngle, maxZAxisClampAngle, gameObject.transform);
            }
        }
    }
}