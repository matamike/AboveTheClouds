using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Singleton<PlayerController> {
    public static event EventHandler<OnStatusChangedEventArgs> OnStatusChanged;
    public static event EventHandler<OnVelocityChangedEventArgs> OnVelocityChanged;
    public class OnStatusChangedEventArgs : EventArgs {
        public int state;
    }

    public class OnVelocityChangedEventArgs : EventArgs
    {
        public Vector3 velocity;
        public bool grounded;
    }

    
    

    private Rigidbody _rigidbody;
    private bool _isSprinting;
    private bool _isGrounded;
    private bool _isJumping;
    private Vector3 _direction;
    private Vector3 _computedDirection;
    private float _movementSpeed;

    //Parameters
    private float _jumpForce = 7f;
    private float _walkForce = 100f;
    private float _sprintForce = 280f;
    private float _turnDirectionSpeed = 1f;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _isGrounded = true;
    }

    private void OnEnable() {
        InputUtility.OnMovePerformed += InputUtility_OnMovePerformed;
        InputUtility.OnSprintPerformed += InputUtility_OnSprintPerformed;
        InputUtility.OnJumpPerformed += InputUtility_OnJumpPerformed;
    }

    private void OnDisable() {
        InputUtility.OnMovePerformed -= InputUtility_OnMovePerformed;
        InputUtility.OnSprintPerformed -= InputUtility_OnSprintPerformed;
        InputUtility.OnJumpPerformed -= InputUtility_OnJumpPerformed;
    }

    //Event Hook Callbacks
    private void InputUtility_OnSprintPerformed(object sender, InputUtility.OnSprintPerformedEventArgs e) => _isSprinting = e.sprint;
    private void InputUtility_OnMovePerformed(object sender, InputUtility.OnMovePerformedEventArgs e) => _direction = e.direction.normalized;
    private void InputUtility_OnJumpPerformed(object sender, InputUtility.OnJumpPerformedEventArgs e){
        if (_isGrounded) _isJumping = e.jump; //elligible for jump only if grounded.
    }
    private void Update() {
        UpdateControlStatus();

        //Motion Control.
        if (_direction != Vector3.zero) {
            _computedDirection = GetMappedCameraDirection();
            Turn(_computedDirection);
            TransformPlayerDirection();
        }
    }

    private void FixedUpdate(){
        OnVelocityChanged?.Invoke(this, new OnVelocityChangedEventArgs{
            velocity = _rigidbody.velocity,
            grounded = _isGrounded
        });

        if(_direction != Vector3.zero && _isGrounded && !_isJumping) Move();
    }

    private void OnCollisionStay(Collision collision) => _isGrounded = true;
    private void OnCollisionExit(Collision collision) => _isGrounded = false;
    public bool IsMoving() => (_direction != Vector3.zero) ? true : false;

    private void UpdateControlStatus(){
        //TODO Rewrite and Clean up
        if(_isGrounded && _isJumping){
            Jump();
            OnStatusChanged?.Invoke(this, new OnStatusChangedEventArgs { state = 3 });
            _isJumping = false;
            return;
        }
        if (_isSprinting && _direction != Vector3.zero) OnStatusChanged?.Invoke(this, new OnStatusChangedEventArgs { state = 2 });
        else{
            if (_direction != Vector3.zero) OnStatusChanged?.Invoke(this, new OnStatusChangedEventArgs { state = 1 });
            else OnStatusChanged?.Invoke(this, new OnStatusChangedEventArgs { state = 0 });
        }
    }
    
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

    private void Jump(){
        _rigidbody.AddExplosionForce(_jumpForce, transform.position, 1f, 1f, ForceMode.Impulse);
    }

    private void Move(){
        if (_isSprinting) _movementSpeed = Mathf.Lerp(_movementSpeed, _sprintForce, 2f);
        else _movementSpeed = Mathf.Lerp(_movementSpeed, _walkForce, 2f);

        _rigidbody.velocity = new Vector3(transform.forward.x * _movementSpeed * Time.deltaTime,
                                        _rigidbody.velocity.y,
                                        transform.forward.z * _movementSpeed * Time.deltaTime);
    }

    private void Turn(Vector3 lookDirection){
        lookDirection.y = 0f; // no movement in Y Axis
        Quaternion rotation = Quaternion.LookRotation(lookDirection, transform.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _turnDirectionSpeed);
    }

    private void TransformPlayerDirection(){
        _computedDirection = Vector3.RotateTowards(_direction, transform.forward, 180f * Mathf.Deg2Rad, 1f);
    }
}