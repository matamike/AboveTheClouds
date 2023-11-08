using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUtility : MonoBehaviour{
    public static InputUtility Instance {  get; private set; }

    private InputSystem _inputSystem;
    public static event EventHandler<OnMovePerformedEventArgs> OnMovePerformed;
    public static event EventHandler<OnSprintPerformedEventArgs> OnSprintPerformed;

    public class OnMovePerformedEventArgs : EventArgs {
        public Vector3 direction;
    }

    public class OnSprintPerformedEventArgs: EventArgs{
        public bool sprint; 
    }

    private void Awake(){
        if(Instance == null || Instance != this) Instance = this;
        //optional in the future DontDestroyOnLoad etc.
    }

    private void OnEnable(){
        if(_inputSystem == null) _inputSystem = new InputSystem(); // Create an Instance.
        _inputSystem.Game.Enable(); //Enable

        //Subscribe to Input System Events
        _inputSystem.Game.Move.performed += InputSystem_Move_performed;
        _inputSystem.Game.Move.canceled += InputSystem_Move_canceled;
        _inputSystem.Game.Sprint.performed += InputSystem_Sprint_performed;
        _inputSystem.Game.Sprint.canceled += InputSystem_Sprint_canceled;
    }
    private void OnDisable(){
        _inputSystem.Game.Move.performed -= InputSystem_Move_performed;
        _inputSystem.Game.Move.canceled -= InputSystem_Move_canceled;
        _inputSystem.Game.Sprint.performed -= InputSystem_Sprint_performed;
        _inputSystem.Game.Sprint.canceled -= InputSystem_Sprint_canceled;
        if (_inputSystem != null) _inputSystem.Game.Disable(); // Disable
    }


    // Event Listeners
    private void InputSystem_Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        OnSprintPerformed?.Invoke(this, new OnSprintPerformedEventArgs{ sprint = false });
    }

    private void InputSystem_Sprint_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        OnSprintPerformed?.Invoke(this, new OnSprintPerformedEventArgs{ sprint = true });
    }

    private void InputSystem_Move_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        OnMovePerformed?.Invoke(this, new OnMovePerformedEventArgs { direction = Vector3.zero });
    }

    private void InputSystem_Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        OnMovePerformed?.Invoke(this, new OnMovePerformedEventArgs { direction = new Vector3(obj.ReadValue<Vector2>().x, 0f, obj.ReadValue<Vector2>().y) });
    }
}