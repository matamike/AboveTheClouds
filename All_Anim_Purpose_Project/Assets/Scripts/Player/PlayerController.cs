using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Singleton<PlayerController> {
    public static event EventHandler<OnStatusChangedEventArgs> OnStatusChanged;
    public static event EventHandler<OnVelocityChangedEventArgs> OnVelocityChanged;

    //Event Args Templates
    public class OnStatusChangedEventArgs : EventArgs {
        public int state;
    }
    public class OnVelocityChangedEventArgs : EventArgs{
        public Vector3 velocity;
        public bool grounded;
    }

    private Rigidbody _rigidbody;
    private bool _isSprinting;
    private bool _isGrounded;
    private bool _isJumping;
    private Vector3 _direction; //calculated
    private float _movementSpeed; // calculated

    //Parameters
    private float _jumpForce = 6f;
    private float _walkForce = 140f;
    private float _sprintForce = 340f;
    private float _turnDirectionSpeed = 1f;
    private float waitTime = 0.2f;
    private float lerpDirectionTime = 10f;


    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
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
        if(IsGrounded()) _isJumping = e.jump;
    }
    private void Update() { 
        ControlDirection(); // Turning the player
        ControlMovement(); // Moving the player
        BroadCastMotionStatus(); //Broadcast to Animator State
        BroadCastVelocity(); //Broadcast Rigidbody Velocity (along with ground state)    
    }

    private void OnCollisionEnter(Collision collision) => StartCoroutine(DelayDirection());

    private void OnCollisionStay(Collision other){
        Vector3 closestPoint = other.collider.ClosestPointOnBounds(transform.position);
        bool state = CheckGrounded(closestPoint, transform.position);
        _isGrounded = state;
        _isJumping = !state;
        if (_isJumping){
            if (Mathf.Abs(_rigidbody.velocity.y) < 0.1f){
                _isGrounded = true;
                _isJumping = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision) => StopCoroutine(DelayDirection());

    public bool HasMovingDirection() => (_direction != Vector3.zero) ? true : false;
    public bool IsSprinting() => (IsWalking() && _isSprinting) ? true : false;
    public bool IsWalking() => (_direction != Vector3.zero) ? true : false;
    public bool IsJumping() => _isJumping;
    public bool IsGrounded() => _isGrounded;
    public bool IsFalling() =>  (_rigidbody.velocity.y < -0.1f) ? true : false;

    private bool CheckGrounded(Vector3 contactPoint, Vector3 comparePosition) => (Vector3.Distance(contactPoint, comparePosition) < 0.1f) ? true : false;
    private void BroadCastVelocity(){
        OnVelocityChanged?.Invoke(this, new OnVelocityChangedEventArgs{
            velocity = _rigidbody.velocity,
            grounded = _isGrounded
        });
    }
    private void BroadCastMotionStatus(){
        // Jump State
        if (IsJumping() || IsFalling()){
            OnStatusChanged?.Invoke(this, new OnStatusChangedEventArgs { state = 3 });
            return;
        }

        // Walk / Run
        if (IsWalking())
        {
            if(IsSprinting()) OnStatusChanged?.Invoke(this, new OnStatusChangedEventArgs { state = 2 });
            else OnStatusChanged?.Invoke(this, new OnStatusChangedEventArgs { state = 1 });
        }
        else{
            //Idle
            OnStatusChanged?.Invoke(this, new OnStatusChangedEventArgs { state = 0 });
        }
    }
    private void ControlMovement(){
        Move();
        Jump();
    }
    private void ControlDirection(){
        if (HasMovingDirection()) Turn();
    }
    private void Jump(){
        if (IsGrounded() && IsJumping()) _rigidbody.AddExplosionForce(_jumpForce, transform.position, 1f, 1f, ForceMode.Impulse);
    }
    private void Move(){
        if (HasMovingDirection() && IsGrounded() && !IsJumping()){
            UpdateMovementSpeed();
            _rigidbody.velocity = new Vector3(transform.forward.x * _movementSpeed * Time.deltaTime,
                                            _rigidbody.velocity.y,
                                            transform.forward.z * _movementSpeed * Time.deltaTime);
        }
    }
    private void UpdateMovementSpeed(){
        float targetSpeed = _isSprinting ? _sprintForce : _walkForce;
        _movementSpeed = Mathf.Lerp(_movementSpeed, targetSpeed, 2f);
    }
    private void Turn(){
        Vector3 _targetLookDirection = GetMappedCameraDirection();
        _targetLookDirection.y = 0f; // no movement in Y Axis
        Quaternion rotation = Quaternion.LookRotation(_targetLookDirection, transform.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _turnDirectionSpeed);
    }
    private Vector3 GetMappedCameraDirection()
    {
        if (_direction == Vector3.zero) return Vector3.zero;

        if (_direction == Vector3.forward) return CameraFollow.Instance.GetCameraForward();
        else if (_direction == Vector3.back) return CameraFollow.Instance.GetCameraBack();
        else if (_direction == Vector3.up) return CameraFollow.Instance.GetCameraUp();
        else if (_direction == Vector3.down) return CameraFollow.Instance.GetCameraDown();
        else if (_direction == Vector3.right) return CameraFollow.Instance.GetCameraRight();
        else if (_direction == Vector3.left) return CameraFollow.Instance.GetCameraLeft();
        else
        {
            if (_direction.z > 0)
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

    private IEnumerator DelayDirection(){
        _direction = Vector3.zero;
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(LerpDirection());
    }

    private IEnumerator LerpDirection(){
        _direction = Vector3.Lerp(_direction, InputUtility.Instance.GetLastKnownDirection(), lerpDirectionTime);
        yield return new WaitForSeconds(lerpDirectionTime);
    }
}