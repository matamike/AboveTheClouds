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

    private float YAxisVelocity = 0.0f;
    private bool _grounded = true;

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

    private void PlayerController_OnVelocityChanged(object sender, PlayerController.OnVelocityChangedEventArgs e){
        YAxisVelocity = e.velocity.y;
        _animator.SetFloat("JumpVelocity", YAxisVelocity);
        _grounded = e.grounded;

        // Jump Reset
        if (_animator.GetBool("IsJumping") && _grounded && Mathf.Approximately(YAxisVelocity, 0f)){
            _animator.SetBool("IsJumping", false);
            _animator.SetFloat("JumpVelocity", 0.0f);
        }
        else if(YAxisVelocity < -1f) _animator.SetBool("IsJumping", true); // Fall state

    }
    private void PlayerController_OnStatusChanged(object sender, PlayerController.OnStatusChangedEventArgs e) => SetState((AnimState)e.state);


    //Animation State Functions
    private void SetState(AnimState state) => _state = state;

    private void CheckAnimationState(){
        //TODO Rewrite and Clean up
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