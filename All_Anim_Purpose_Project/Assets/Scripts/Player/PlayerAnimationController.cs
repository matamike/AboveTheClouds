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
    }
    private AnimState _state;
    private Animator _animator;

    private void Awake(){
        _animator = GetComponent<Animator>();
    }

    private void Start(){
        _state = AnimState.Idle; // default state
        PlayerController.OnStatusChanged += PlayerController_OnStatusChanged;
    }

    private void PlayerController_OnStatusChanged(object sender, PlayerController.OnStatusChangedEventArgs e) => SetState((AnimState)e.state);

    private void Update(){
        CheckAnimationState();
    }

    //Animation State Functions
    private void SetState(AnimState state) => _state = state;

    private void CheckAnimationState(){
        switch (_state){
            case AnimState.Idle:
                _animator.SetBool("IsRunning", false);
                _animator.SetBool("IsWalking", false);
                break;
            case AnimState.Walk:
                _animator.SetBool("IsRunning", false);
                _animator.SetBool("IsWalking", true);
                break;
            case AnimState.Run:
                _animator.SetBool("IsWalking", false);
                _animator.SetBool("IsRunning", true);
                break;
        }
    }
}