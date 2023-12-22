using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Utility.PlaceUtility.PlaceLoadingUtility;

public class GridManager : Singleton<GridManager>{
    [Range(0f, 10f)] private float extraOffset = 1f;
    [SerializeField] private GridPoolObjectSO[] gridPoolObjectList; //change to accept GridPoolObjectSO instead of GO
    [SerializeField] private Vector3 startingGridPosition;
    private List<TileGrid> _grids = new List<TileGrid>();
    private float baseOffset;
    private int _checkPointReached = -1;
    private int _finalGridCount = -1;

    private void OnEnable(){
        SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
        CheckPoint.OnCheckPointReached += CheckPoint_OnCheckPointReached;
    }

    private void OnDisable(){
        SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
        CheckPoint.OnCheckPointReached -= CheckPoint_OnCheckPointReached;
    }

    private void OnDestroy(){
        DestroyAllGrids();
    }

    //Event Hooks
    private void SceneManager_OnSceneLoaded(Scene arg0, LoadSceneMode arg1){
        switch (GetCurrentPlace()){
            case Place.ObstacleCourse:
                //Difficulty Based (Random)
                DifficultyPresetSO difficultyPresetSO = LevelUtility.GetActiveDifficultyPreset();
                if (difficultyPresetSO != null){
                    _finalGridCount = difficultyPresetSO.GetGridCount(); //The length of the list (to have).
                    gridPoolObjectList = difficultyPresetSO.GetGridPoolObjectSOList().ToArray();
                    if (gridPoolObjectList.Length == 0) return;
                    CreateGrid(difficultyPresetSO.GetGridSizeX(), difficultyPresetSO.GetGridSizeY());
                    return;
                }
                //User Defined Difficulty Custom Template
                UserDefinedMappedDifficultySO userDefinedMappedDifficultySO = LevelUtility.GetActiveUserDefinedMappedDifficulty();
                if (userDefinedMappedDifficultySO != null){
                    _finalGridCount = userDefinedMappedDifficultySO.GetTemplateGridCount();
                    GameObject[,] mapping = userDefinedMappedDifficultySO.GetConvertedTileMapToGameObjects();
                    CreatePredefinedGrid(mapping);
                    return;
                }
                break;
        }
    }

    private void CheckPoint_OnCheckPointReached(object sender, CheckPoint.OnCheckPointReachedEventArgs e) {
        startingGridPosition += e.self.position;
        _checkPointReached = e.index;
        DestroyGrid(0);
        if (_checkPointReached < (_finalGridCount - 1)) {
            DifficultyPresetSO difficultyPresetSO = LevelUtility.GetActiveDifficultyPreset();
            gridPoolObjectList = difficultyPresetSO.GetGridPoolObjectSOList().ToArray();
            CreateGrid(difficultyPresetSO.GetGridSizeX(), difficultyPresetSO.GetGridSizeY());
        }
        else if(_checkPointReached == (_finalGridCount - 1)){
            Debug.Log("Won");
            MyGameManager.Instance.TeleportPlayerBackToHub();
            DestroyAllGrids();
        }
    }
    /////////////
    
    //Member Functions

    private void DestroyAllGrids(){
        if (_grids.Count == 0) return;
        for(int i = _grids.Count - 1; i >= 0; i--) DestroyGrid(i);
    }

    private void DestroyGrid(int gridIndex){
        if (_grids.Count > 0 && _grids[gridIndex] != null){
            _grids[gridIndex].DestroyGridElements();
            _grids.RemoveAt(gridIndex);
            GC.Collect();
        }
    }

    public void RequestDestroyGrid(int index) => DestroyGrid(index);

    public void CreateGrid(int sizeX, int sizeY, bool hasCheckPoint = true){
        baseOffset = CalculateBasePositionOffset();
        List<GameObject> pool = new List<GameObject>();
        foreach (GridPoolObjectSO gridPoolObjectSO in gridPoolObjectList) pool.Add(gridPoolObjectSO.PoolObject);

        TileGrid tileGrid = new TileGrid(sizeX, sizeY, baseOffset + extraOffset, true, pool.ToArray(), startingGridPosition);
        _grids.Add(tileGrid);
        if(hasCheckPoint) CreateGridCheckpoint(_grids.Count-1); 
    }

    public void CreatePredefinedGrid(GameObject[,] userDefinedTileMapping, bool hasCheckPoint = true){
        baseOffset = CalculateBasePositionOffset();
        TileGrid tileGrid = new TileGrid(baseOffset + extraOffset, true, userDefinedTileMapping, startingGridPosition);
        _grids.Add(tileGrid);
        if (hasCheckPoint) CreateGridCheckpoint(_grids.Count - 1);
    }

    private void CreateGridCheckpoint(int index = -1){
        if (_grids[index] is not null){
            Vector2 gridSize = _grids[index].GetGridSize();
            float tileSize = _grids[index].GetGridTileSize();

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            float depthOffset = 6f;
            float sideSizeOffset = (gridSize.x * tileSize);
            go.transform.localScale = new Vector3(sideSizeOffset, 1f, depthOffset);
            go.transform.position = startingGridPosition + new Vector3((gridSize.x * tileSize) / 2f, 0f, baseOffset + depthOffset + (gridSize.y * tileSize));
            CheckPoint checkpoint = go.AddComponent<CheckPoint>();
            checkpoint.SetCheckPointIndex(_checkPointReached + 1);
        }
    }

    //private void UpdateGrid(int index = -1){
    //    if (_grids[index] is not null){
    //        baseOffset = CalculateBasePositionOffset();
    //        _grids[index].UpdateStartingPosition(startingGridPosition);
    //        _grids[index].UpdateGridOffset(baseOffset + extraOffset, true);
    //    }
    //}

    private float CalculateBasePositionOffset(){
        float averageOffset = 0f;
        if (gridPoolObjectList.Length > 0){
            foreach (GridPoolObjectSO gridPoolObjectSO in gridPoolObjectList){
                GameObject item = gridPoolObjectSO.PoolObject;
                float scaleSize = 0.0f;
                if (item != null) scaleSize = (item.transform.lossyScale.x + item.transform.lossyScale.z) / 2;
                averageOffset += scaleSize;
            }
            averageOffset /= gridPoolObjectList.Length;
        }
        return averageOffset;
    }
}