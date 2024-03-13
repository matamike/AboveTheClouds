using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Singleton<PlayerController> {
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

    //Variables for checking fall and wait time before next motion
    private bool waitBeforeMovingAgain = false;

    //Parameters
    private float _jumpForce = 3f; //3f-4f (optimal range)
    private float _walkForce = 140f;
    private float _sprintForce = 340f;
    private float _turnDirectionSpeed = 1f;
    

    // Collision Parameters
    [SerializeField]private List<GameObject> _collidingObjects;
    private List<Tuple<int, int>> _contactPoints;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _collidingObjects = new List<GameObject>();
        _contactPoints = new List<Tuple<int, int>>();
    }

    private void OnEnable() {
        DispatcherUtility.OnRemoveRequest += DispatcherUtility_OnSpecificMemberRequest;
        InputManager.OnMovePerformed += InputUtility_OnMovePerformed;
        InputManager.OnSprintPerformed += InputUtility_OnSprintPerformed;
        InputManager.OnJumpPerformed += InputUtility_OnJumpPerformed;
        InputManager.OnSpecialMovePerformed += InputUtility_OnSpecialMovePerformed;
    }

    

    private void OnDisable() {
        DispatcherUtility.OnRemoveRequest -= DispatcherUtility_OnSpecificMemberRequest;
        InputManager.OnMovePerformed -= InputUtility_OnMovePerformed;
        InputManager.OnSprintPerformed -= InputUtility_OnSprintPerformed;
        InputManager.OnJumpPerformed -= InputUtility_OnJumpPerformed;
        InputManager.OnSpecialMovePerformed -= InputUtility_OnSpecialMovePerformed;
    }

    //Event Hook Callbacks
    private void DispatcherUtility_OnSpecificMemberRequest(object sender, DispatcherUtility.SpecificMemberEventArgs e){
        if (e._receiver == gameObject) ForceRemoveCollidingElement(e._sender);
    }
    private void InputUtility_OnSprintPerformed(object sender, InputManager.OnSprintPerformedEventArgs e) => _isSprinting = e.sprint;
    private void InputUtility_OnMovePerformed(object sender, InputManager.OnMovePerformedEventArgs e) => _direction = e.direction.normalized;
    private void InputUtility_OnJumpPerformed(object sender, InputManager.OnJumpPerformedEventArgs e){
        if(IsGrounded()) _isJumping = e.jump;
    }
    private void InputUtility_OnSpecialMovePerformed(object sender, InputManager.OnSpecialMovePerformedEventArgs e){
        //Debug.Log("Special Move ID: " + e.special_id);
        //TODO IMPLEMENT FUTURE !!!
    }
    private void Update() {  
        ControlDirection(); // Turning the player
        ClampPlayerVelocity(); //Clamp Velocity of Player Rigidbody to limits
    }

    private void FixedUpdate(){
        ControlMovement(); // Moving the player
        BroadCastVelocity(); //Broadcast Rigidbody Velocity (along with ground state)
        if (_rigidbody.velocity.y > 1f || _rigidbody.velocity.y < -1f){
            _isGrounded = false; //airborne (falling or jumping -> any case)
        }

        if (_rigidbody.velocity.y > 4f){
            PlayerSoundController.Instance.PlayPlayerSFX(PlayerSoundController.PLAYER_SFX_TYPE.JUMP);
        }

        ControlVelocity();
    }

    private void OnCollisionEnter(Collision other){
        TryAddCollidingElement(other.gameObject);
        if (other.gameObject.TryGetComponent(out IInteractable interactable)){
            IInteractable.OnInteractorPositionChanged?.Invoke(this, new IInteractable.OnInteractEventArgs{
                position = transform.position,
            });
        }
    }

    private void OnCollisionStay(Collision other){
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

    public void ForceRemoveCollidingElement(GameObject source) => TryRemoveCollidingElement(source);
    private void TryAddCollidingElement(GameObject go){
        if (!_collidingObjects.Contains(go)){
            _collidingObjects.Add(go);
            _contactPoints.Add(new Tuple<int, int>(0, 0));
        }
        else
        {
            int collidingIndex = _collidingObjects.IndexOf(go);
            _contactPoints[collidingIndex] = new Tuple<int, int>(0, 0);
        }
    }
    private void TryRemoveCollidingElement(GameObject go){
        if (_collidingObjects.Contains(go)){
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
            Vector3 normal = other.contacts[i].normal;
            float angle = Mathf.Abs(Vector3.SignedAngle(normal, transform.up, Vector3.up));
            if (angle >= 0f && angle < 60f){
                Debug.DrawRay(other.contacts[i].point, normal * 3f, Color.green, 2f);
                groundedPointsCount++;
            }
            else{
                Debug.DrawRay(other.contacts[i].point, normal * 3f, Color.red, 2f);
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
    public bool IsFalling() => (_rigidbody.velocity.y < - 1f) ? true : false;  
    private void BroadCastVelocity(){
        OnVelocityChanged?.Invoke(this, new OnVelocityChangedEventArgs{
            velocity = _rigidbody.velocity,
            grounded = _isGrounded
        });
    }
    private void ControlMovement(){
        if (!waitBeforeMovingAgain){
            Move();
            Jump();
        }
    }

    private void ControlVelocity(){
        float currentMultiplier = TimeMultiplierUtility.GetTimeMultiplier();
        _rigidbody.velocity *= currentMultiplier;
        if(currentMultiplier == 0 && _rigidbody.useGravity) _rigidbody.useGravity = false;
        if (currentMultiplier != 0 && !_rigidbody.useGravity) _rigidbody.useGravity = true;
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
            PlayerSoundController.Instance.PlayPlayerSFX(PlayerSoundController.PLAYER_SFX_TYPE.WALK);
        }
        else
        {
            if (!HasMovingDirection()) _rigidbody.velocity = Vector3.LerpUnclamped(_rigidbody.velocity, Vector3.zero, 0.1f * Time.fixedDeltaTime);
            if (IsJumping()) _rigidbody.velocity = Vector3.LerpUnclamped(_rigidbody.velocity, new Vector3(_rigidbody.velocity.x, 0f,_rigidbody.velocity.z), 0.1f * Time.fixedDeltaTime);
        }
    }
    private void UpdateMovementSpeed(){
        float targetSpeed = _isSprinting ? _sprintForce : _walkForce;
        _movementSpeed = Mathf.Lerp(_movementSpeed, targetSpeed, 2f * TimeMultiplierUtility.GetTimeMultiplier());

        //Set Audio Source Pitch Level (Walk/Sprint)
        if(targetSpeed == _sprintForce) {
            PlayerSoundController.Instance.SetPitch(1.2f);
        }
        else{
            PlayerSoundController.Instance.SetPitch(1.0f);
        }
    }
    private void Turn(){
        Vector3 _targetLookDirection = GetMappedCameraDirection();
        _targetLookDirection.y = 0f; // no movement in Y Axis
        Quaternion rotation = Quaternion.LookRotation(_targetLookDirection, transform.up);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, _turnDirectionSpeed * TimeMultiplierUtility.GetTimeMultiplier());
    }
    private Vector3 GetMappedCameraDirection()
    {
        if (_direction == Vector3.zero) return Vector3.zero;

        if (_direction == Vector3.forward) return CameraController.Instance.GetCameraForward();
        else if (_direction == Vector3.back) return CameraController.Instance.GetCameraBack();
        else if (_direction == Vector3.up) return CameraController.Instance.GetCameraUp();
        else if (_direction == Vector3.down) return CameraController.Instance.GetCameraDown();
        else if (_direction == Vector3.right) return CameraController.Instance.GetCameraRight();
        else if (_direction == Vector3.left) return CameraController.Instance.GetCameraLeft();
        else
        {
            if (_direction.z > 0)
            {
                if (_direction.x > 0) return CameraController.Instance.GetCameraForwardRight();
                else return CameraController.Instance.GetCameraForwardLeft();
            }
            else
            {
                if (_direction.x > 0) return CameraController.Instance.GetCameraBackRight();
                else return CameraController.Instance.GetCameraBackLeft();
            }
        }
    }
}