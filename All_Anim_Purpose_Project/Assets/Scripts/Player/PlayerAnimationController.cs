using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour{
    private Animator _animator;

    private void Awake(){
        _animator = GetComponent<Animator>();
    }

    private void OnEnable(){
        PlayerController.OnVelocityChanged += PlayerController_OnVelocityChanged;
    }

    private void OnDisable(){
        PlayerController.OnVelocityChanged -= PlayerController_OnVelocityChanged;
    }


    private void PlayerController_OnVelocityChanged(object sender, PlayerController.OnVelocityChangedEventArgs e) => SetVelocityAnimParameters(e.grounded, e.velocity);

    //Animation State Functions
    private void SetVelocityAnimParameters(bool grounded, Vector3 velocity){
        ToggleJumpState(!grounded);
        SetMotionVelocityAnimParameter(velocity.magnitude);
        SetJumpVelocityAnimParameter(velocity.y);
    }

    private void SetJumpVelocityAnimParameter(float yAxisVelocity) => _animator.SetFloat("JumpVelocity", yAxisVelocity);
    private void SetMotionVelocityAnimParameter(float velocityMagnitude) => _animator.SetFloat("MovingVelocity", velocityMagnitude);
    private void ToggleJumpState(bool state) => _animator.SetBool("IsJumping", state);
}