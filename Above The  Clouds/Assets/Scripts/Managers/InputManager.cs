using System;
using UnityEngine;

public class InputManager : Singleton<InputManager>{
    private InputSystem _inputSystem;
    public static event EventHandler<OnMovePerformedEventArgs> OnMovePerformed;
    public static event EventHandler<OnSprintPerformedEventArgs> OnSprintPerformed;
    public static event EventHandler<OnJumpPerformedEventArgs> OnJumpPerformed;
    public static event EventHandler<OnSpecialMovePerformedEventArgs> OnSpecialMovePerformed;

    public class OnSpecialMovePerformedEventArgs : EventArgs{
        public int special_id;
    }

    public class OnMovePerformedEventArgs : EventArgs {
        public Vector3 direction;
    }

    public class OnSprintPerformedEventArgs: EventArgs{
        public bool sprint; 
    }

    public class OnJumpPerformedEventArgs : EventArgs{
        public bool jump;
    }

    private bool controlsLocked = false;

    private void OnEnable(){
        if(_inputSystem == null) _inputSystem = new InputSystem(); // Create an Instance.
        _inputSystem.Game.Enable(); //Enable

        //Subscribe to Input System Events
        _inputSystem.Game.Move.performed += InputSystem_Move_performed;
        _inputSystem.Game.Move.canceled += InputSystem_Move_canceled;
        _inputSystem.Game.Sprint.performed += InputSystem_Sprint_performed;
        _inputSystem.Game.Sprint.canceled += InputSystem_Sprint_canceled;
        _inputSystem.Game.Jump.performed += InputSystem_Jump_performed;
        _inputSystem.Game.SpecialMove_1.performed += InputSystem_SpecialMove_1_performed;
        _inputSystem.Game.SpecialMove_2.performed += InputSystem_SpecialMove_2_performed;
        _inputSystem.Game.SpecialMove_3.performed += InputSystem_SpecialMove_3_performed;
        _inputSystem.Game.SpecialMove_4.performed += InputSystem_SpecialMove_4_performed;
        IUICursorToggle.OnCursorShow += IUICursorToggle_CursorShow;
        IUICursorToggle.OnCursorHide += IUICursorToggle_CursorHide;
    }

    private void OnDisable(){
        _inputSystem.Game.Move.performed -= InputSystem_Move_performed;
        _inputSystem.Game.Move.canceled -= InputSystem_Move_canceled;
        _inputSystem.Game.Sprint.performed -= InputSystem_Sprint_performed;
        _inputSystem.Game.Sprint.canceled -= InputSystem_Sprint_canceled;
        _inputSystem.Game.Jump.performed -= InputSystem_Jump_performed;
        _inputSystem.Game.SpecialMove_1.performed -= InputSystem_SpecialMove_1_performed;
        _inputSystem.Game.SpecialMove_2.performed -= InputSystem_SpecialMove_2_performed;
        _inputSystem.Game.SpecialMove_3.performed -= InputSystem_SpecialMove_3_performed;
        _inputSystem.Game.SpecialMove_4.performed -= InputSystem_SpecialMove_4_performed;
        IUICursorToggle.OnCursorShow -= IUICursorToggle_CursorShow;
        IUICursorToggle.OnCursorHide -= IUICursorToggle_CursorHide;

        if (_inputSystem != null) _inputSystem.Game.Disable(); // Disable
    }

    public void SetControlLockStatus(bool flag){
        controlsLocked = flag;
        if(controlsLocked) OnMovePerformed?.Invoke(this, new OnMovePerformedEventArgs { direction = Vector3.zero });
    }

    // Event Listeners
    private void InputSystem_Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        OnSprintPerformed?.Invoke(this, new OnSprintPerformedEventArgs{ sprint = false });
    }

    private void InputSystem_Sprint_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        if (!controlsLocked) OnSprintPerformed?.Invoke(this, new OnSprintPerformedEventArgs{ sprint = true });
    }

    private void IUICursorToggle_CursorHide(object sender, EventArgs e) => SetControlLockStatus(false);
    private void IUICursorToggle_CursorShow(object sender, EventArgs e) => SetControlLockStatus(true);

    private void InputSystem_SpecialMove_4_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!controlsLocked) OnSpecialMovePerformed?.Invoke(this, new OnSpecialMovePerformedEventArgs { special_id = 4 });
    }

    private void InputSystem_SpecialMove_3_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!controlsLocked) OnSpecialMovePerformed?.Invoke(this, new OnSpecialMovePerformedEventArgs { special_id = 3 });
    }

    private void InputSystem_SpecialMove_2_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!controlsLocked) OnSpecialMovePerformed?.Invoke(this, new OnSpecialMovePerformedEventArgs { special_id = 2 });
    }

    private void InputSystem_SpecialMove_1_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!controlsLocked) OnSpecialMovePerformed?.Invoke(this, new OnSpecialMovePerformedEventArgs { special_id = 1 });
    }

    private void InputSystem_Move_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        OnMovePerformed?.Invoke(this, new OnMovePerformedEventArgs { direction = Vector3.zero });
    }

    private void InputSystem_Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        if (!controlsLocked) OnMovePerformed?.Invoke(this, new OnMovePerformedEventArgs { direction = new Vector3(obj.ReadValue<Vector2>().x, 0f, obj.ReadValue<Vector2>().y) });
    }

    private void InputSystem_Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj){
        if (!controlsLocked) OnJumpPerformed?.Invoke(this, new OnJumpPerformedEventArgs { jump = true });
    }
}