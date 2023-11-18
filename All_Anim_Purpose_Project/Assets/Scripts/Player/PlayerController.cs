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
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isJumping;
    private Vector3 _direction; //calculated
    private float _movementSpeed; // calculated

    //Parameters
    [SerializeField] private float _jumpForce = 6f;
    private float _walkForce = 140f;
    private float _sprintForce = 340f;
    private float _turnDirectionSpeed = 1f;
    private float waitTime = 0.2f;
    private float lerpDirectionTime = 10f;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable() {
        PlayerInputManager.OnMovePerformed += InputUtility_OnMovePerformed;
        PlayerInputManager.OnSprintPerformed += InputUtility_OnSprintPerformed;
        PlayerInputManager.OnJumpPerformed += InputUtility_OnJumpPerformed;
    }

    private void OnDisable() {
        PlayerInputManager.OnMovePerformed -= InputUtility_OnMovePerformed;
        PlayerInputManager.OnSprintPerformed -= InputUtility_OnSprintPerformed;
        PlayerInputManager.OnJumpPerformed -= InputUtility_OnJumpPerformed;
    }

    //Event Hook Callbacks
    private void InputUtility_OnSprintPerformed(object sender, PlayerInputManager.OnSprintPerformedEventArgs e) => _isSprinting = e.sprint;
    private void InputUtility_OnMovePerformed(object sender, PlayerInputManager.OnMovePerformedEventArgs e) => _direction = e.direction.normalized;
    private void InputUtility_OnJumpPerformed(object sender, PlayerInputManager.OnJumpPerformedEventArgs e){
        if(IsGrounded()) _isJumping = e.jump;
    }
    private void Update() { 
        ControlDirection(); // Turning the player
        ControlMovement(); // Moving the player
        ClampPlayerVelocity(); //Clamp Velocity of Player Rigidbody to limits
        BroadCastMotionStatus(); //Broadcast to Animator State
        BroadCastVelocity(); //Broadcast Rigidbody Velocity (along with ground state)    
    }

    private void OnCollisionEnter(Collision collision){
        //_isGrounded = true;
    }
    private void OnCollisionStay(Collision other){
        bool state = CheckGrounded(other.contacts);
        //Debug.Log("Check Grounded" + state);
        _isGrounded = state;
        _isJumping = !state;
        if (_isJumping){
            if (Mathf.Abs(_rigidbody.velocity.y) < 0.25f){
                _isGrounded = true;
                _isJumping = false;
            }
        }

        //transform.parent = other.transform.root;
    }

    private void OnCollisionExit(Collision collision){
        if(_rigidbody.velocity.y > 0.5f) _isGrounded = false;
        transform.parent = null;
    }

    private void ClampPlayerVelocity()
    {
        _rigidbody.velocity = new Vector3(Mathf.Clamp(_rigidbody.velocity.x, -50f, 50f),
                                          Mathf.Clamp(_rigidbody.velocity.y, -10f, 10f),
                                          Mathf.Clamp(_rigidbody.velocity.z, -50f, 50f));

        if (_rigidbody.velocity.y == 50f) _isJumping = false;
    }
    public bool HasMovingDirection() => (_direction != Vector3.zero) ? true : false;
    public bool IsSprinting() => (IsWalking() && _isSprinting) ? true : false;
    public bool IsWalking() => (_direction != Vector3.zero) ? true : false;
    public bool IsJumping() => _isJumping;
    public bool IsGrounded() => _isGrounded;
    public bool IsFalling() =>  (_rigidbody.velocity.y < -0.1f) ? true : false;

    private bool CheckGrounded(ContactPoint[] contactPoints){
        bool state = false;
        foreach(ContactPoint point in contactPoints){
            float angle = Mathf.Abs(Vector3.SignedAngle(transform.position, point.point, Vector3.up));
            //Debug.Log("Angle" + angle);
            if (angle >=0f && angle <=60f) state = true;
            else state = false;
        }

        return state;
    }
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
        else
        {
            if (!HasMovingDirection()) _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, 1f * Time.deltaTime);
            if (IsJumping()) _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, new Vector3(_rigidbody.velocity.x, 0f,_rigidbody.velocity.z), 1f * Time.deltaTime);
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
        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, _turnDirectionSpeed);
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
        _direction = Vector3.Lerp(_direction, PlayerInputManager.Instance.GetLastKnownDirection(), lerpDirectionTime);
        yield return new WaitForSeconds(lerpDirectionTime);
    }
}