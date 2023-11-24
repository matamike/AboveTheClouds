using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class TileGrid{
    public static event EventHandler<OnPositionChangedEventArgs> OnPositionChanged;

    public class OnPositionChangedEventArgs: EventArgs
    {
        public GameObject self;
        public Vector3 target;
    }

    private int _width;
    private int _height;
    private GameObject[,] _gridArray;
    private float _tileOffset;
    private Vector3 _startingPosition;

    public TileGrid(int xSize, int ySize , float offset, bool isSmooth, GameObject[] prefabs, Vector3 startingPosition = default) {
        if (xSize < 0 || ySize < 0) return;
        if (prefabs == null || prefabs.Length == 0) return;
        _width = xSize;
        _height = ySize;
        _gridArray = new GameObject[xSize, ySize];
        _tileOffset = offset;
        _startingPosition = startingPosition;
        CreateGrid(prefabs, isSmooth);
    }

    ~TileGrid(){
        Debug.Log("DESTRUCTOR FOR TILEGRID WAS CALLED DAMN IT!");
        foreach(var item in _gridArray) UnityEngine.Object.Destroy(item.gameObject);
    }

    private void CreateGrid(GameObject[] prefabs, bool isSmooth = false)
    {
        int seed = URandom.Range(-9999, 9999);
        URandom.InitState(seed);

        for (int x = 0; x < _width; x++){
            for(int y = 0; y < _height; y++){
                URandom.State newState = URandom.state;
                GameObject prefab = prefabs[URandom.Range(0, prefabs.Length)];
                _gridArray[x, y] = CreateGridElement(prefab,x, y);
                MoveGridElementToPosition(x, y, isSmooth);
            }
        }
    }

    private GameObject CreateGridElement(GameObject prefab, int x, int y){
        GameObject element = UnityEngine.Object.Instantiate(prefab, _startingPosition, Quaternion.identity);
        element.name = prefab.name+"("+ x +"_"+ y +")";
        return element;
    }

    public bool GridElementExists(int x, int y) => (_gridArray[x, y] is not null) ? true : false;

    public Vector2Int GetTileIndices(GameObject element){
        for(int x= 0; x < _width; x++){
            for(int y=0; y < _height; y++){
                if (_gridArray[x, y] != null && _gridArray[x, y] == element) return new Vector2Int(x, y);
            }
        }
        return new Vector2Int(-1, -1);
    }

    public void UpdateGridOffset(float offset, bool isSmoothChange = false)
    {
        _tileOffset = offset;
        for (int x = 0; x < _width; x++){
            for (int y = 0; y < _height; y++){
                MoveGridElementToPosition(x, y, isSmoothChange);
            }
        }
    }

    public void UpdateStartingPosition(Vector3 newStartingPosition) => _startingPosition = newStartingPosition;

    public Vector2 GetGridSize() => new Vector2(_width, _height);

    public float GetGridTileSize() => (_gridArray[0, 0].transform.lossyScale.x + _gridArray[0, 0].transform.lossyScale.z) / 2f;

    public void DestroyGridElements(){
        foreach (var item in _gridArray){
            if (item != null) UnityEngine.Object.Destroy(item.gameObject);
        }
    }
    private void MoveGridElementToPosition(int x, int y, bool isSmooth = default){
        if (!isSmooth) _gridArray[x, y].gameObject.transform.position = _startingPosition + new Vector3(x * _tileOffset, 0f, y * _tileOffset);
        else{
            Vector3 targetPosition = _startingPosition + new Vector3(x * _tileOffset, 0f, y * _tileOffset);
            OnPositionChanged?.Invoke(this, new OnPositionChangedEventArgs {
                self = _gridArray[x,y],
                target = targetPosition,
            });
        }
    }
}