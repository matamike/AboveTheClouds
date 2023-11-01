using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyGrid{
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

    public MyGrid(int xSize, int ySize , float offset, bool isSmooth, GameObject prefab) {
        if (xSize < 0 || ySize < 0) return;
        if (prefab == null) return;

        _width = xSize;
        _height = ySize;
        _gridArray = new GameObject[xSize, ySize];
        _tileOffset = offset;
        CreateGrid(prefab, isSmooth);
    }

    
    private void CreateGrid(GameObject prefab, bool isSmooth = false){
        for(int x = 0; x < _width; x++)
        {
            for(int y = 0; y < _height; y++)
            {
                _gridArray[x, y] = CreateGridElement(prefab,x, y);
                MoveGridElementToPosition(x, y, isSmooth);
            }
        }
    }

    private GameObject CreateGridElement(GameObject prefab, int x, int y){
        GameObject element = new GameObject("GridElement", typeof(MeshFilter), typeof(MeshRenderer), typeof(Move), typeof(BoxCollider));
        element.GetComponent<MeshFilter>().sharedMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
        element.GetComponent<MeshRenderer>().sharedMaterial = prefab.GetComponent<MeshRenderer>().sharedMaterial;
        element.name = "GridElement("+x+"_"+y+")";
        return element;
    }

    public void UpdateGridOffset(float offset, bool isSmoothChange = false)
    {
        _tileOffset = offset;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                MoveGridElementToPosition(x, y, isSmoothChange);
            }
        }
    }

    private void MoveGridElementToPosition(int x, int y, bool isSmooth = false)
    {
        if (!isSmooth)
        {
            _gridArray[x, y].gameObject.transform.position = new Vector3(x * _tileOffset, 0f, y * _tileOffset);
        }
        else
        {
            Vector3 targetPosition = new Vector3(x * _tileOffset, 0f, y * _tileOffset);
            OnPositionChanged?.Invoke(this, new OnPositionChangedEventArgs
            {
                self = _gridArray[x,y],
                target = targetPosition,
            });
        }
    }
}
