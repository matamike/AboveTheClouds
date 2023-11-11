using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodypartsController : MonoBehaviour{
    delegate void BodyPart(Transform bodypart, Vector3 euler);

    [SerializeField] Transform _bodyPartTransform;
    private BodyPart _bodyPart;
    [SerializeField] bool isHandled = true;
    float zAxisAngleThresholdInDegrees = 0f;

    private Vector3 lastKnownDirection;

    private void Awake(){
        _bodyPart = FaceDirection;
    }

    private void Start(){
        CameraFollow.OnDirectionChanged += CameraFollow_OnDirectionChanged;
    }

    private void CameraFollow_OnDirectionChanged(object sender, CameraFollow.OnDirectionChangedEventArgs e) => _bodyPart(_bodyPartTransform, e.euler);

    private void FaceDirection(Transform bodypart, Vector3 euler){
        if (isHandled){
            bool isMoving = PlayerController.Instance.IsMoving();
            Vector3 lookAtForwardDirection = CameraFollow.Instance.GetCameraForward();
            Vector3 lookAtUp = transform.up; 

            float dotProductPlayerCamera = Mathf.Round(Vector3.Dot(lookAtForwardDirection, transform.parent.forward));

            if (isMoving) bodypart.LookAt(transform.position + transform.parent.forward, transform.parent.up);
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
