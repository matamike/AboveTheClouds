using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour{
    private TileGrid _tileGridAssigned;

    private void OnDisable(){
        _tileGridAssigned.OnGridDestroying -= TileGrid_OnGridDestroying;
    }

    private void TileGrid_OnGridDestroying(object sender, EventArgs e) => Destroy(gameObject);

    public void AssignTileGrid(TileGrid tileGrid){
        _tileGridAssigned = tileGrid;
        _tileGridAssigned.OnGridDestroying += TileGrid_OnGridDestroying;
    }
}