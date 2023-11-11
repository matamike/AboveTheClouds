using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Singleton<PlayerController>{
    public static event EventHandler<OnStatusChangedEventArgs> OnStatusChanged;
    public class OnStatusChangedEventArgs : EventArgs{
       public int state;
    }


    [SerializeField] private Transform _DebugRayParentTransform; //Ray Origin
    [SerializeField][Range(1f, 100f)] private float _movementSpeed = 2f;
    [SerializeField][Range(0f, 1f)] private float _turnDirectionSpeed = 1f;
        
    private Rigidbody _rigidbody;
    private bool _isSprinting; //event acquired
    [SerializeField]private Vector3 _direction = Vector3.zero; //event acquired
    private Vector3 _computedDirection;

    private void Start(){
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable(){
        InputUtility.OnMovePerformed += InputUtility_OnMovePerformed;
        InputUtility.OnSprintPerformed += InputUtility_OnSprintPerformed;
    }

    private void OnDisable(){
        InputUtility.OnMovePerformed -= InputUtility_OnMovePerformed;
        InputUtility.OnSprintPerformed -= InputUtility_OnSprintPerformed;
    }

    //Event Hook Callbacks
    private void InputUtility_OnSprintPerformed(object sender, InputUtility.OnSprintPerformedEventArgs e) => _isSprinting = e.sprint;
    private void InputUtility_OnMovePerformed(object sender, InputUtility.OnMovePerformedEventArgs e) => _direction = e.direction.normalized;
    private void Update(){
        if (_direction != Vector3.zero){
            _computedDirection = GetMappedCameraDirection();
            Turn(_computedDirection);
            TransformPlayerDirection();
        }
        UpdateControlStatus();
    }

    private void FixedUpdate(){
        //if(_direction != Vector3.zero){
            //_rigidbody.AddForce(_computedDirection * _movementSpeed, ForceMode.Force);
        //}
    }

    private void UpdateControlStatus(){
        if (_isSprinting) OnStatusChanged?.Invoke(this, new OnStatusChangedEventArgs { state = 2 });
        else{
            if (_direction != Vector3.zero) OnStatusChanged?.Invoke(this, new OnStatusChangedEventArgs { state = 1 });
            else OnStatusChanged?.Invoke(this, new OnStatusChangedEventArgs { state = 0 });
        }
    }

    public bool IsMoving() => (_direction != Vector3.zero) ? true : false;
    
    private Vector3 GetMappedCameraDirection(){
        if (_direction == Vector3.zero) return Vector3.zero;

        if (_direction == Vector3.forward) return CameraFollow.Instance.GetCameraForward();
        else if (_direction == Vector3.back) return CameraFollow.Instance.GetCameraBack();
        else if (_direction == Vector3.right) return CameraFollow.Instance.GetCameraRight();
        else if(_direction == Vector3.left) return CameraFollow.Instance.GetCameraLeft();
        else{ 
            if(_direction.z > 0)
            {
                if (_direction.x > 0) return CameraFollow.Instance.GetCameraForwardRight();
                else return CameraFollow.Instance.GetCameraForwardLeft();
            }
            else
            {
                if (_direction.x > 0) return CameraFollow.Instance.GetCameraBackRight();
                else return CameraFollow.Instance.GetCameraBackLeft();
            }
        }
    }

    private void Turn(Vector3 lookDirection){
        lookDirection.y = 0f; // no movement in Y Axis
        Quaternion rotation = Quaternion.LookRotation(lookDirection, transform.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _turnDirectionSpeed);
    }

    private void TransformPlayerDirection(){
        Vector3 playerForward = transform.forward;
        _computedDirection = Vector3.RotateTowards(_direction, playerForward, 180f * Mathf.Deg2Rad, 1f);
    }
    private void DebugDirectionRay(Vector3 dir, Color color, float duration) => Debug.DrawRay((transform.position), dir * 10f, color, duration);
}