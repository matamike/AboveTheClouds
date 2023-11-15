using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour{
    //Anim State enum
    public enum AnimState
    {
        Idle,
        Walk,
        Run,
        Jump,
    }
    private AnimState _state;
    private Animator _animator;
    private Vector3 _rigidbodyVelocity;


    private void Awake(){
        _animator = GetComponent<Animator>();
    }

    private void Start(){
        _state = AnimState.Idle; // default state
        PlayerController.OnStatusChanged += PlayerController_OnStatusChanged;
    }

    private void OnEnable(){
        PlayerController.OnStatusChanged += PlayerController_OnStatusChanged;
        PlayerController.OnVelocityChanged += PlayerController_OnVelocityChanged;
    }

    private void OnDisable(){
        PlayerController.OnStatusChanged -= PlayerController_OnStatusChanged;
        PlayerController.OnVelocityChanged -= PlayerController_OnVelocityChanged;
    }

    private void Update(){
        CheckAnimationState();
    }

    private void PlayerController_OnVelocityChanged(object sender, PlayerController.OnVelocityChangedEventArgs e) => SetVelocity(e.grounded, e.velocity);
    private void PlayerController_OnStatusChanged(object sender, PlayerController.OnStatusChangedEventArgs e) => SetState((AnimState)e.state);

    //Animation State Functions
    private void SetJumpVelocity(bool grounded){
        _animator.SetFloat("JumpVelocity", _rigidbodyVelocity.y);

        // Jump Reset
        if (grounded && Mathf.Approximately(_rigidbodyVelocity.y, 0f)){
            _animator.SetBool("IsJumping", false);
            _animator.SetFloat("JumpVelocity", 0.0f);
        }
        else if (_rigidbodyVelocity.y < -1f) _animator.SetBool("IsJumping", true); // Fall state
    }

    private void SetMotionVelocity() => _animator.SetFloat("MovingVelocity", _rigidbodyVelocity.magnitude);

    private void SetVelocity(bool grounded, Vector3 velocity){
        _rigidbodyVelocity = velocity;
        _rigidbodyVelocity = new Vector3(_rigidbodyVelocity.x, Mathf.Clamp(Mathf.Round(_rigidbodyVelocity.y), -1f, 1f), _rigidbodyVelocity.z);

        SetJumpVelocity(grounded);
        SetMotionVelocity();
    }

    private void SetState(AnimState state) => _state = state;
   
    private void CheckAnimationState(){
        switch (_state)
        {
            case AnimState.Idle:
                _animator.SetBool("IsRunning", false);
                _animator.SetBool("IsWalking", false);
                break;
            case AnimState.Walk:
                _animator.SetBool("IsRunning", false);
                _animator.SetBool("IsWalking", true);
                break;
            case AnimState.Run:
                if(!_animator.GetBool("IsWalking"))_animator.SetBool("IsWalking", true);
                _animator.SetBool("IsRunning", true);
                break;
            case AnimState.Jump:
                _animator.SetBool("IsJumping", true);
                break;
        }
    }
}