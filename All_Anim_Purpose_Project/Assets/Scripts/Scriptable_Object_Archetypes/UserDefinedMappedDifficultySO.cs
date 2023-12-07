using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class UserDefinedMappedDifficultySO : ScriptableObject {
    [SerializeField] private List<GameObject> tilePool;
    [SerializeField][Range(2, 10)] private int tileMapSizeX = 3, tileMapSizeY = 3;
    private readonly int gridCount = 1;
    private TileType.Type[,] tileMappingTileTypes; //mapping of the types in grid.
    private GameObject[,] tileMappingGameObjects; //gameobject collection of Tiletype.Type grid.

    //Saved Template (Deconstructed)
    [SerializeField] private List<Vector2Int> gridTileIndices;  //saved indices of last saved in template
    [SerializeField] private List<TileType.Type> gridTileTypes; // saved types of last saved in template

    public int GetTemplateGridCount() => (gridCount <= 0) ? 1 : gridCount; //default (1)

    //Create/Get enum 2d Array mapping
    //public void SetGridTileMap(int w, int h) => tileMappingTileTypes = new TileType.Type[w, h];
    //public TileType.Type[,] GetGridTileMap() => tileMappingTileTypes;

    //Set/Get Specific index in enum 2d Array mapping
    //public void SetGridTileMapValue(int xPosition, int yPosition, TileType.Type value) => tileMappingTileTypes[xPosition, yPosition] = value;
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
                int tileType = (int)tileMappingTileTypes[x, y];
                tileMappingGameObjects[x, y] = tilePool[tileType];
            }
        }   
        return tileMappingGameObjects;
    }
    public void SaveTemplateMapping(GameObject[,] mapping){
        Debug.Log("Attempt to save changes");
        tileMappingGameObjects = mapping; //save mapping to GOs

        // Assign to types
        for(int x = 0; x < tileMappingGameObjects.GetLength(0); x++){
            for(int y= 0; y < tileMappingGameObjects.GetLength(1); y++){
                int indexOfPool = tilePool.IndexOf(tileMappingGameObjects[x, y]);
                tileMappingTileTypes[x, y] = (TileType.Type)indexOfPool;
            }
        }
        //Save changes from 2D array to containers.
        SaveMapping();
    }
    private void SaveMapping(){
        if (tileMappingTileTypes != null){
            Debug.Log("Save changes to mapping " + this.name);
            //Clear only if it contains something before assigning the newly updated mapping.
            if (gridTileIndices.Count > 0) gridTileIndices.Clear();
            if (gridTileTypes.Count > 0) gridTileTypes.Clear();

            //assign new size of the 2d array of TileType.Type enum
            tileMapSizeX = tileMappingTileTypes.GetLength(0);
            tileMapSizeY = tileMappingTileTypes.GetLength(1);

            //Store the new values in the 2d array into the containers respectively.
            for (int x = 0; x < tileMapSizeX; x++){
                for (int y = 0; y < tileMapSizeY; y++){
                    gridTileIndices.Add(new Vector2Int(x, y));
                    gridTileTypes.Add(tileMappingTileTypes[x, y]);
                }
            }
        }
        else{
            Debug.Log("Uninitialized 2d array container for mappings in template : " + this.name);
        }
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