using System;
using UnityEngine;

public class CursorVisibilityUtility : IUICursorToggle{
    public static void SetCursorVisibility(bool flag){
        Cursor.visible = flag;
        if (Cursor.visible){
            IUICursorToggle.OnShow?.Invoke(null, EventArgs.Empty);
        }
        else{
            IUICursorToggle.OnHide?.Invoke(null, EventArgs.Empty);
        }
    }
}
