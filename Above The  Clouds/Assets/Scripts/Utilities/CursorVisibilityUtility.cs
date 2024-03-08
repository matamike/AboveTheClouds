using System;
using UnityEngine;

public class CursorVisibilityUtility : IUICursorToggle{
    public static void BroadCastToEntity(object invokerType){
        Cursor.visible = true;
        if (Cursor.visible){
            Cursor.lockState = CursorLockMode.Confined;
            IUICursorToggle.OnCursorShow?.Invoke(invokerType, EventArgs.Empty);
        }
        else{
            Cursor.lockState = CursorLockMode.Locked;
            IUICursorToggle.OnCursorHide?.Invoke(invokerType, EventArgs.Empty);
        }
        IUICursorToggle.OnToggle?.Invoke(invokerType, EventArgs.Empty);
    }

    public static void ForceCloseAllEntities(object invokerType) {
        Cursor.visible = false;
        if (Cursor.visible){
            Cursor.lockState = CursorLockMode.Confined;
            IUICursorToggle.OnCursorShow?.Invoke(invokerType, EventArgs.Empty);
        }
        else{
            Cursor.lockState = CursorLockMode.Locked;
            IUICursorToggle.OnCursorHide?.Invoke(invokerType, EventArgs.Empty);
        }
        IUICursorToggle.OnForceClose?.Invoke(invokerType, EventArgs.Empty);
    }

    public static void SetCursorVisibility(bool flag){
        Cursor.visible = flag;
        if (Cursor.visible){
            Cursor.lockState = CursorLockMode.Confined;
            IUICursorToggle.OnCursorShow?.Invoke(null, EventArgs.Empty);
        }
        else{
            Cursor.lockState = CursorLockMode.Locked;
            IUICursorToggle.OnCursorHide?.Invoke(null, EventArgs.Empty);
        }
    }
}
