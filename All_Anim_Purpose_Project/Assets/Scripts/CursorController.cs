using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour{

    public event EventHandler<OnObjectFocusEventArgs> OnObjectFocusGained;
    public event EventHandler<OnObjectFocusEventArgs> OnObjectFocusLost;

    public class OnObjectFocusEventArgs : EventArgs
    {
       public GameObject focus;
    }

    public static CursorController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        MouseUtility.SetMousePosition(mousePosition);
    }

    public void CheckRaycast()
    {   
        GameObject go = MouseUtility.GetMouseToWorldRayHit();
        if (go != null) OnObjectFocusGained?.Invoke(this, new OnObjectFocusEventArgs
        {
            focus = go,
        });
        else OnObjectFocusLost?.Invoke(this, new OnObjectFocusEventArgs
        {
            focus = null,
        });
    }
}
