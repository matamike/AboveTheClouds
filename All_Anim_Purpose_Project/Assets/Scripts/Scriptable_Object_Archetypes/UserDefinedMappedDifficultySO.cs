using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class UserDefinedMappedDifficultySO : ScriptableObject {
    [SerializeField] private List<GridPoolObjectSO> tilePool;
    [SerializeField][Range(2, 10)] private int tileMapSizeX = 3, tileMapSizeY = 3;
    private readonly int gridCount = 1;
    private TileType.Type[,] tileMappingTileTypes; //mapping of the types in grid.
    private GameObject[,] tileMappingGameObjects; //gameobject collection of Tiletype.Type grid.

    //Saved Template (Deconstructed)
    [SerializeField] private List<Vector2Int> gridTileIndices;  //saved indices of last saved in template
    [SerializeField] private List<TileType.Type> gridTileTypes; // saved types of last saved in template

    private GameObject GetTilePrefab(TileType.Type tileType){
        GameObject go = null;
        foreach(var tile in tilePool){
            if (tile.TileType == tileType) return tile.PoolObject;
        }

        return go;
    }

    public int GetTemplateGridCount() => (gridCount <= 0) ? 1 : gridCount; //default (1)

    //Set/Get Specific index in enum 2d Array mapping
    public void SaveNewGridTileMapValue(int xPosition, int yPosition, TileType.Type value){
        //save to permanent mapping.
        int index = gridTileIndices.IndexOf(new Vector2Int(xPosition, yPosition));
        if (gridTileIndices[index] != null){
            gridTileTypes[index] = value;
        }
        //Apply Changes to both TileType.Tile 2D array and GameObject 2D Array (During play).
        tileMappingTileTypes[xPosition,yPosition] = value;
        tileMappingGameObjects[xPosition, yPosition] = GetTilePrefab(tileMappingTileTypes[xPosition, yPosition]);
    }
    public TileType.Type GetGridTileMapValue(int xPosition, int yPosition) => tileMappingTileTypes[xPosition, yPosition];

    public Color GetGridTileMapColor(int xPosition, int yPosition) {
        if (gridTileIndices.Contains(new Vector2Int(xPosition, yPosition))){
            int index = gridTileIndices.IndexOf(new Vector2Int(xPosition, yPosition));
            Color color = TileTypeUtility.GetTypeColor(gridTileTypes[index]);
            return color;
        }
        else return Color.black;
    }
    public GameObject[,] GetConvertedTileMapToGameObjects(){
        tileMappingGameObjects = new GameObject[tileMapSizeX, tileMapSizeY];
        for(int x = 0; x < tileMapSizeX; x++){
            for(int y = 0; y < tileMapSizeY; y++){
                tileMappingGameObjects[x, y] = GetTilePrefab(tileMappingTileTypes[x, y]);
            }
        }   
        return tileMappingGameObjects;
    }

    private void LoadMapping(){
        //Initialize Size of actual data container that we use in the game.
        tileMappingTileTypes = new TileType.Type[tileMapSizeX, tileMapSizeY];

        //load the designated data through the different (serializable) data containers
        if (gridTileIndices.Count > 0 && gridTileTypes.Count > 0){
            Debug.Log("Load last saved Mapping for " + this.name);
            for (int i = 0; i < gridTileIndices.Count; i++){
                tileMappingTileTypes[gridTileIndices[i].x, gridTileIndices[i].y] = gridTileTypes[i];
            }
        }
    }
    private void CreateInitialMapping(){
        //Initialize Containers if uninitialized
        if(gridTileIndices == null || gridTileTypes == null){
            gridTileIndices = new List<Vector2Int>();
            gridTileTypes = new List<TileType.Type>();
        }

        //Store a default mapping of tileMapSizeX and tileMapSizeY into the containers.
        if (gridTileIndices.Count == 0 && gridTileTypes.Count == 0){
            Debug.Log("Attempt to initialize template " + this.name);
            for (int x = 0; x < tileMapSizeX; x++){
                for (int y = 0; y < tileMapSizeY; y++){
                    gridTileIndices.Add(new Vector2Int(x, y));
                    gridTileTypes.Add(TileType.Type.Standard);
                }
            }
        }
    }
    public void ActivateTemplate(){
        CreateInitialMapping();
        LoadMapping();
        EditorUtility.SetDirty(this);
    }
}