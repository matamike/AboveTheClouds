using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : Singleton<CursorController>{
    public void DebugPrintCursorPosition() => Debug.Log("Cursor Position: " + Input.mousePosition);
}
