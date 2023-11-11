using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : Singleton<CameraFollow>{
    public static event EventHandler<OnDirectionChangedEventArgs> OnDirectionChanged;
    public class OnDirectionChangedEventArgs : EventArgs {
        public Vector3 euler;
    }

    [SerializeField] private Transform _rotateAroundTransform;
    [SerializeField][Range(1f, 10f)] private float rotationSpeed = 2f;
    [SerializeField] private Transform _followFocusTransform;
    [SerializeField][Range(0f, 1f)] private float followSpeed = 0.25f;
    [SerializeField][Range(1f, 10f)] private float followDistance = 6f;
    [SerializeField][Range(0f, -79f)] private float _XAxisAngleMinThreshold = -35f;
    [SerializeField][Range(0f, 79f)] private float _XAxisAngleMaxThreshold = 35f;
    private Vector3 _targetVelocity = Vector3.zero;
    private Vector3 _calculatedOffset;
    private float _lastKnownMousePositionXAxis, _lastKnownMousePositionYAxis;
    //Directions
    private Vector3 _cameraForward = Vector3.zero;
    private Vector3 _cameraBack = Vector3.zero;
    private Vector3 _cameraLeft = Vector3.zero;
    private Vector3 _cameraRight = Vector3.zero;
    private Vector3 _cameraForwardRight = Vector3.zero;
    private Vector3 _cameraForwardLeft = Vector3.zero;
    private Vector3 _cameraBackLeft = Vector3.zero; 
    private Vector3 _cameraBackRight = Vector3.zero;


    private void Start(){
        _targetVelocity = Vector3.zero;
        CalculateOffset();
    }

    private void Update(){
        RotateAroundTarget();
    }

    private void LateUpdate(){
        FollowTarget();
        UpdateDirections();
    }

    private void UpdateDirections(){
        if(_cameraForward != transform.forward) {
            _cameraForward = transform.forward;
            _cameraRight = transform.right;
            _cameraLeft = -transform.right;
            _cameraBack = -transform.forward;
            _cameraForwardRight = (transform.forward + transform.right).normalized;
            _cameraForwardLeft = (transform.forward + (-transform.right)).normalized;
            _cameraBackLeft = (-transform.forward + (-transform.right)).normalized;
            _cameraBackRight = (-transform.forward + (transform.right)).normalized;
        }
        OnDirectionChanged?.Invoke(this, new OnDirectionChangedEventArgs { euler = transform.eulerAngles });
    }
    public Vector3 GetCameraForward() => _cameraForward;
    public Vector3 GetCameraBack() => _cameraBack;
    public Vector3 GetCameraLeft() => _cameraLeft;
    public Vector3 GetCameraRight() => _cameraRight;
    public Vector3 GetCameraForwardLeft() => _cameraForwardLeft;
    public Vector3 GetCameraForwardRight() => _cameraForwardRight;
    public Vector3 GetCameraBackLeft() => _cameraBackLeft;
    public Vector3 GetCameraBackRight() => _cameraBackRight;

    private void FollowTarget(){
        Vector3 targetPosition = _followFocusTransform.position + _calculatedOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _targetVelocity, followSpeed);
    }
    private void CalculateOffset() => _calculatedOffset = (transform.position - _followFocusTransform.position).normalized * followDistance;
    private void RotateAroundTarget(){
        CalculateOffset();
        //Retrieve normalized mouse X,Y Axis Values
        _lastKnownMousePositionXAxis = MouseUtility.GetMouseXNormalized();
        _lastKnownMousePositionYAxis = MouseUtility.GetMouseYNormalized();

        //Calculate offset (Apply Negative spectrum)
        float xRotationEulerAngles;
        if(transform.eulerAngles.x > 0f && transform.eulerAngles.x < 80f) xRotationEulerAngles = transform.eulerAngles.x;
        else xRotationEulerAngles = transform.eulerAngles.x - 360f;

        //Rotation X,Y Axeses
        transform.RotateAround(_rotateAroundTransform.position, transform.up, _lastKnownMousePositionXAxis * rotationSpeed); //Mouse X (Y Axis - Rotation)

        //Mouse Y (X Axis - Rotation)
        if ((xRotationEulerAngles > 0f && xRotationEulerAngles < _XAxisAngleMaxThreshold) || (xRotationEulerAngles < 0f && xRotationEulerAngles > _XAxisAngleMinThreshold)){
            transform.RotateAround(_rotateAroundTransform.position, transform.right, _lastKnownMousePositionYAxis * rotationSpeed); 
        }
        else //Limits Reached
        {
            if(xRotationEulerAngles > _XAxisAngleMaxThreshold && _lastKnownMousePositionYAxis < 0f)
                transform.RotateAround(_rotateAroundTransform.position, transform.right, _lastKnownMousePositionYAxis * rotationSpeed);

            if (xRotationEulerAngles < _XAxisAngleMinThreshold && _lastKnownMousePositionYAxis > 0f)
                        transform.RotateAround(_rotateAroundTransform.position, transform.right, _lastKnownMousePositionYAxis * rotationSpeed);
        }

        //Face Target
        transform.LookAt(_rotateAroundTransform, Vector3.up);
    }
}