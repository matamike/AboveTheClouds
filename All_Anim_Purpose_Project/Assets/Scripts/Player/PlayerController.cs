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
    private float _jumpForce = 8f;
    private float _walkForce = 140f;
    private float _sprintForce = 340f;
    private float _turnDirectionSpeed = 1f;

    // Collision Parameters
    [SerializeField] private List<GameObject> _collidingObjects;
    [SerializeField] private List<Tuple<int, int>> _contactPoints;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _collidingObjects = new List<GameObject>();
        _contactPoints = new List<Tuple<int, int>>();
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
        ClampPlayerVelocity(); //Clamp Velocity of Player Rigidbody to limits
        BroadCastMotionStatus(); //Broadcast to Animator State
        BroadCastVelocity(); //Broadcast Rigidbody Velocity (along with ground state)
        if(_rigidbody.velocity.y > 1f || _rigidbody.velocity.y < -1f) _isGrounded = false;
    }

    private void FixedUpdate(){   
        ControlMovement(); // Moving the player
    }

    private void OnCollisionStay(Collision other){
        TryAddCollidingElement(other.gameObject);
        ComputeGroundedPoints(other);
        CheckGroundedWithCollidingObjects();

        //Attempt to interact with Interactables
        if (other.gameObject.TryGetComponent(out IInteractable interactable)){
            interactable?.Interact(gameObject);
        }
    }

    private void OnCollisionExit(Collision other){
        TryRemoveCollidingElement(other.gameObject);

        //Attempt to cancel Interaction with Interactables
        if (other.gameObject.TryGetComponent(out IInteractable interactable)){
            interactable?.CancelInteracion(gameObject);
        }
    }

    private void TryAddCollidingElement(GameObject go){
        if (!_collidingObjects.Contains(go)){
            //add 1st time.
            _collidingObjects.Add(go);
            _contactPoints.Add(new Tuple<int, int>(0, 0));
        }
        else{
            //existing
            int collidingIndex = _collidingObjects.IndexOf(go);
            _contactPoints[collidingIndex] = new Tuple<int, int>(0, 0);
        }
    }
    private void TryRemoveCollidingElement(GameObject go){
        if (_collidingObjects.Contains(go)){
            //remove when collision stops entirely.
            int index = _collidingObjects.IndexOf(go);
            _collidingObjects.RemoveAt(index);
            _contactPoints.RemoveAt(index);
        }
    }

    //computes the contact points from incoming collision whether the angle difference is between 0-60 or more
    private void ComputeGroundedPoints(Collision other){
        int groundedPointsCount = 0, rest = 0;
        int index = _collidingObjects.IndexOf(other.gameObject);
        for (int i = 0; i < other.GetContacts(other.contacts); i++){
            float angle = Mathf.Abs(Vector3.SignedAngle(other.contacts[i].normal, transform.up, Vector3.up));
            if (angle >= 0f && angle < 60f){
                //Debug.DrawRay(other.contacts[i].point, normal * 3f, Color.green, 2f);
                groundedPointsCount++;
            }
            else{
                //Debug.DrawRay(other.contacts[i].point, normal * 3f, Color.red, 2f);
                rest++;
            }
        }
        _contactPoints[index] = new Tuple<int, int>(groundedPointsCount, rest);
    }
    private void CheckGroundedWithCollidingObjects(){
        int timesFoundCollisionWithGround = 0;
        for (int i = 0; i < _collidingObjects.Count; i++){
            if (_contactPoints[i].Item1 > 0) timesFoundCollisionWithGround += 1;
        }
        _isGrounded = (timesFoundCollisionWithGround > 0) ? true : false;
    }

    private void ClampPlayerVelocity(){
        _rigidbody.velocity = new Vector3(Mathf.Clamp(_rigidbody.velocity.x, -15f, 15f),
                                          Mathf.Clamp(_rigidbody.velocity.y, -10f, 10f),
                                          Mathf.Clamp(_rigidbody.velocity.z, -15f, 15f));
    }
    public bool HasMovingDirection() => (_direction != Vector3.zero) ? true : false;
    public bool IsSprinting() => (IsWalking() && _isSprinting) ? true : false;
    public bool IsWalking() => (_direction != Vector3.zero) ? true : false;
    public bool IsJumping() => _isJumping;
    public bool IsGrounded() => _isGrounded;
    public bool IsFalling(){

        if (_rigidbody.velocity.y > 1f || _rigidbody.velocity.y < -1f) Debug.Log("Fall: (Velocity (Y) ):" +_rigidbody.velocity.y);
        return (_rigidbody.velocity.y < - 1f) ? true : false;
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
        if (IsGrounded() && IsJumping()){
            _rigidbody.AddExplosionForce(_jumpForce, transform.position, 1f, 1f, ForceMode.Impulse);
            _isJumping = false;
        }
    }
    private void Move(){
        if (HasMovingDirection() && IsGrounded() && !IsJumping()){
            UpdateMovementSpeed();
            _rigidbody.velocity = new Vector3(transform.forward.x * _movementSpeed * Time.fixedDeltaTime,
                                            _rigidbody.velocity.y,
                                            transform.forward.z * _movementSpeed * Time.fixedDeltaTime);
        }
        else
        {
            if (!HasMovingDirection()) _rigidbody.velocity = Vector3.LerpUnclamped(_rigidbody.velocity, Vector3.zero, 0.1f * Time.fixedDeltaTime);
            if (IsJumping()) _rigidbody.velocity = Vector3.LerpUnclamped(_rigidbody.velocity, new Vector3(_rigidbody.velocity.x, 0f,_rigidbody.velocity.z), 0.1f * Time.fixedDeltaTime);
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
}