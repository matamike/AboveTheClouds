using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour{
    public static CursorController Instance { get; private set; }

    private void Awake(){
        Instance = this;
    }
}
