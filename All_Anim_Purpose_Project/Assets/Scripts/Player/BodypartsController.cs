using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodypartsController : MonoBehaviour{
    delegate void BodyPart(Transform bodypart);

    [SerializeField] Transform _bodyPartTransform;
    [SerializeField] bool isHandled = true;
    
    private BodyPart _bodyPart;
    private Vector3 lastKnownDirection;
    private float zAxisAngleThresholdInDegrees = 0f;

    private void Awake(){
        _bodyPart = FaceDirection;
    }

    private void OnEnable(){
        CameraFollow.OnDirectionChanged += CameraFollow_OnDirectionChanged;
    }

    private void OnDisable()
    {
        CameraFollow.OnDirectionChanged -= CameraFollow_OnDirectionChanged;
    }

    private void CameraFollow_OnDirectionChanged(object sender, EventArgs e) => _bodyPart(_bodyPartTransform);

    private void FaceDirection(Transform bodypart){
        if (isHandled){
            Vector3 lookAtForwardDirection = CameraFollow.Instance.GetCameraForward();
            Vector3 lookAtUp = transform.up; 

            float dotProductPlayerCamera = Mathf.Round(Vector3.Dot(lookAtForwardDirection, transform.parent.forward));

            if (PlayerController.Instance.HasMovingDirection()) bodypart.LookAt(transform.position + transform.parent.forward, transform.parent.up);
            else{
                if(dotProductPlayerCamera == 1) lastKnownDirection = CameraFollow.Instance.GetCameraForward();
                else if(dotProductPlayerCamera == -1) lastKnownDirection = CameraFollow.Instance.GetCameraBack();

                bodypart.LookAt(transform.position + lastKnownDirection, lookAtUp);

                //Clamp
                bodypart.localEulerAngles = new Vector3(bodypart.localEulerAngles.x, bodypart.localEulerAngles.y,
                                                        Mathf.Clamp(bodypart.localEulerAngles.z, -zAxisAngleThresholdInDegrees, zAxisAngleThresholdInDegrees));
            }
        }
    }
}
