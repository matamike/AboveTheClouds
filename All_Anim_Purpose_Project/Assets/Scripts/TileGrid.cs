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

    public TileGrid(int xSize, int ySize , float offset, bool isSmooth, GameObject[] prefabs) {
        if (xSize < 0 || ySize < 0) return;
        if (prefabs == null || prefabs.Length == 0) return;
        _width = xSize;
        _height = ySize;
        _gridArray = new GameObject[xSize, ySize];
        _tileOffset = offset;
        CreateGrid(prefabs, isSmooth);
    }

    private void CreateGrid(GameObject[] prefabs, bool isSmooth = false){
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
        GameObject element = UnityEngine.Object.Instantiate(prefab);
        element.name = prefab.name+"("+ x +"_"+ y +")";
        return element;
    }

    public void UpdateGridOffset(float offset, bool isSmoothChange = false){
        _tileOffset = offset;
        for (int x = 0; x < _width; x++){
            for (int y = 0; y < _height; y++){
                MoveGridElementToPosition(x, y, isSmoothChange);
            }
        }
    }

    private void MoveGridElementToPosition(int x, int y, bool isSmooth = false){
        if (!isSmooth) _gridArray[x, y].gameObject.transform.position = new Vector3(x * _tileOffset, 0f, y * _tileOffset);
        else{
            Vector3 targetPosition = new Vector3(x * _tileOffset, 0f, y * _tileOffset);
            OnPositionChanged?.Invoke(this, new OnPositionChangedEventArgs {
                self = _gridArray[x,y],
                target = targetPosition,
            });
        }
    }
}