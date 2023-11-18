using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour{
    public static event EventHandler<OnDestroyTileEventArgs> OnDestroyTile;
    public class OnDestroyTileEventArgs : EventArgs{
        public GameObject tile;
    }
    private void OnDestroy(){
        OnDestroyTile?.Invoke(this, new OnDestroyTileEventArgs{
            tile = gameObject
        });
    }
}