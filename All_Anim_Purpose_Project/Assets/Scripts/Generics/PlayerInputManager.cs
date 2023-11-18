using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : Singleton<PlayerInputManager>{
    private InputSystem _inputSystem;
    public static event EventHandler<OnMovePerformedEventArgs> OnMovePerformed;
    public static event EventHandler<OnSprintPerformedEventArgs> OnSprintPerformed;
    public static event EventHandler<OnJumpPerformedEventArgs> OnJumpPerformed;

    public class OnMovePerformedEventArgs : EventArgs {
        public Vector3 direction;
    }

    public class OnSprintPerformedEventArgs: EventArgs{
        public bool sprint; 
    }

    public class OnJumpPerformedEventArgs : EventArgs{
        public bool jump;
    }

    private Vector3 _lastKnownDirection;

    private void OnEnable(){
        if(_inputSystem == null) _inputSystem = new InputSystem(); // Create an Instance.
        _inputSystem.Game.Enable(); //Enable

        //Subscribe to Input System Events
        _inputSystem.Game.Move.performed += InputSystem_Move_performed;
        _inputSystem.Game.Move.canceled += InputSystem_Move_canceled;
        _inputSystem.Game.Sprint.performed += InputSystem_Sprint_performed;
        _inputSystem.Game.Sprint.canceled += InputSystem_Sprint_canceled;
        _inputSystem.Game.Jump.performed += InputSystem_Jump_performed;
    }

    private void OnDisable(){
        _inputSystem.Game.Move.performed -= InputSystem_Move_performed;
        _inputSystem.Game.Move.canceled -= InputSystem_Move_canceled;
        _inputSystem.Game.Sprint.performed -= InputSystem_Sprint_performed;
        _inputSystem.Game.Sprint.canceled -= InputSystem_Sprint_canceled;
        _inputSystem.Game.Jump.performed += InputSystem_Jump_performed;
        if (_inputSystem != null) _inputSystem.Game.Disable(); // Disable
    }

    public Vector3 GetLastKnownDirection() => _lastKnownDirection;

    // Event Listeners
    private void InputSystem_Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        OnSprintPerformed?.Invoke(this, new OnSprintPerformedEventArgs{ sprint = false });
    }

    private void InputSystem_Sprint_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        OnSprintPerformed?.Invoke(this, new OnSprintPerformedEventArgs{ sprint = true });
    }

    private void InputSystem_Move_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        OnMovePerformed?.Invoke(this, new OnMovePerformedEventArgs { direction = Vector3.zero });
        //_lastKnownDirection = Vector3.zero;
    }

    private void InputSystem_Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        OnMovePerformed?.Invoke(this, new OnMovePerformedEventArgs { direction = new Vector3(obj.ReadValue<Vector2>().x, 0f, obj.ReadValue<Vector2>().y) });
        _lastKnownDirection = new Vector3(obj.ReadValue<Vector2>().x, 0f, obj.ReadValue<Vector2>().y);
    }

    private void InputSystem_Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        OnJumpPerformed?.Invoke(this, new OnJumpPerformedEventArgs { jump = true });
    }
}