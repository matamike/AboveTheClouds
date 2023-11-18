using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>{
    [SerializeField][Range(0f, 10f)] private float extraOffset = 1f;
    [SerializeField] private GameObject[] prefabPool;
    [SerializeField][Range(2,20)] private int gridSizeX, gridSizeY;
    [SerializeField] private Vector3 startingGridPosition;
    [SerializeField]private TileGrid grid;
    private float baseOffset;

    private void OnDestroy(){
        DestroyGrid();
    }

    private void Update(){
        //Create
        if (Input.GetKeyDown(KeyCode.F1)){
            if (grid is null){
                baseOffset = CalculateBasePositionOffset();
                grid = new TileGrid(gridSizeX, gridSizeY, baseOffset + extraOffset, true, prefabPool, startingGridPosition);
            }
        }
        //Update Offset
        if (Input.GetKeyDown(KeyCode.F2)) {
            if (grid is not null){
                baseOffset = CalculateBasePositionOffset();
                grid.UpdateStartingPosition(startingGridPosition);
                grid.UpdateGridOffset(baseOffset + extraOffset, true);
            }
        }
        //Destroy Grid
        if (Input.GetKeyDown(KeyCode.F3)) DestroyGrid();
    }

    private void DestroyGrid(){
        if (grid is not null){
            grid.DestroyGridElements();
            grid = null;
            GC.Collect();
        }
    }

    private float CalculateBasePositionOffset(){
        float averageOffset = 0f;
        if (prefabPool.Length > 0){
            foreach (GameObject item in prefabPool){
                float scaleSize = (item.transform.lossyScale.x + item.transform.lossyScale.z) / 2;
                averageOffset += scaleSize;
            }
            averageOffset /= prefabPool.Length;
        }
        //Debug.Log("Average Offset: " +  averageOffset);
        return averageOffset;
    }
}