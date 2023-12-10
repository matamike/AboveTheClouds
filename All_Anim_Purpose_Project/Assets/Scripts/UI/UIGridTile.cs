using System;
using UnityEngine;

public class UIGridTile : MonoBehaviour{
    private Vector2Int indices;
    private TileType.Type tileType;
    private TileType.Type tempTileType;

    public void SetUnconfirmedTileType(TileType.Type tileType) => tempTileType = tileType;
    public void SetInitialTileType(TileType.Type tileType) => this.tileType = tileType;
    public void ConfirmTileType(){
        if (tempTileType != tileType){
            Debug.Log("Add change to unsaved");
            tileType = tempTileType;
            LevelCreatorUIManager.Instance.AddPendingSaveChange(new Tuple<Vector2Int, TileType.Type>(indices, tileType));
        }
    }
    public void ResetUnconfirmedTileType() => tempTileType = tileType;
    public void SetIndices(int x, int y) => indices = new Vector2Int(x, y);
    public void SetIndices(Vector2Int indices) => this.indices = indices;
    public TileType.Type GetTileType() => tileType;
    public Vector2Int GetIndices() => indices;
}
