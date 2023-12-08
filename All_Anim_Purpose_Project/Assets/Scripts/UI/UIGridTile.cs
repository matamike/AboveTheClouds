using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGridTile : MonoBehaviour{
    private Vector2Int indices;
    private TileType.Type tileType;

    public void SetTileType(TileType.Type tileType) => this.tileType = tileType;
    public void SetIndices(int x, int y) => indices = new Vector2Int(x, y);
    public void SetIndices(Vector2Int indices) => this.indices = indices;
    public TileType.Type GetTileType() => tileType;
    public Vector2Int GetIndices() => indices;
}
