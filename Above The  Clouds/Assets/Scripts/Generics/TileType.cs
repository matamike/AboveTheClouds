using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileType {
    [System.Serializable]
    public enum Type{
        Standard,
        Droppable,
        Rotating,
        Bouncy,
        Empty,
    }
}