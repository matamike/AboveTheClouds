using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Utility.PlaceUtility.PlaceLoadingUtility;

public class GridManager : Singleton<GridManager>{
    [SerializeField][Range(0f, 10f)] private float extraOffset = 1f;
    [SerializeField] private GameObject[] prefabPool;
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

    private void Update(){
    }

    //Event Hooks
    private void SceneManager_OnSceneLoaded(Scene arg0, LoadSceneMode arg1){
        switch (GetCurrentPlace()){
            case Place.ObstacleCourseRandom:
                DifficultyPresetSO difficultyPresetSO = LevelUtility.GetActiveDifficultyPreset();
                _finalGridCount = difficultyPresetSO.GetGridCount(); //The length of the list (to have). 
                prefabPool = difficultyPresetSO.GetTilePool().ToArray();
                Debug.Log("Number of TileGrids for this run : " + _finalGridCount);
                Debug.Log("Sample Pool Size: " + prefabPool.Length);
                Debug.Log("Preset Size X: " + difficultyPresetSO.GetGridSizeX());
                Debug.Log("Preset Size Y: " + difficultyPresetSO.GetGridSizeY());
                CreateGrid(difficultyPresetSO.GetGridSizeX(), difficultyPresetSO.GetGridSizeY());
                break;
                //    case Place.ObstacleCourseUserDefined:
                //        //TODO Implement
                //        break;
                //    case Place.LevelCreator:
                //        //TODO Implement
                //        break;
        }
    }
    private void CheckPoint_OnCheckPointReached(object sender, CheckPoint.OnCheckPointReachedEventArgs e) {
        startingGridPosition += e.self.position;
        _checkPointReached = e.index;
        DestroyGrid(0);
        if (_checkPointReached < (_finalGridCount - 1)) {
            DifficultyPresetSO difficultyPresetSO = LevelUtility.GetActiveDifficultyPreset();
            prefabPool = difficultyPresetSO.GetTilePool().ToArray();
            CreateGrid(difficultyPresetSO.GetGridSizeX(), difficultyPresetSO.GetGridSizeY());
        }
        else if(_checkPointReached == (_finalGridCount - 1)){
            Debug.Log("Won");
            //Update Game Manager of the achievement.
            DestroyAllGrids();
        }
    }
    /////////////
    
    //Member Functions

    private void DestroyAllGrids(){
        if(_grids.Count > 0){
            for(int i = _grids.Count - 1; i >= 0; i--){
                _grids[i].DestroyGridElements();
                _grids.Remove(_grids[i]);
                GC.Collect();
            }
        }
    }

    private void DestroyGrid(int gridIndex){
        if (_grids[gridIndex] != null){
            _grids[gridIndex].DestroyGridElements();
            _grids.RemoveAt(gridIndex);
            GC.Collect();
        }
    }
    public void CreateGrid(int sizeX, int sizeY){
        baseOffset = CalculateBasePositionOffset();
        TileGrid tileGrid = new TileGrid(sizeX, sizeY, baseOffset + extraOffset, true, prefabPool, startingGridPosition);
        _grids.Add(tileGrid);
        CreateGridCheckpoint(_grids.Count-1);
    }

    private void CreateGridCheckpoint(int index = -1){
        if (_grids[index] is not null){
            Vector2 gridSize = _grids[index].GetGridSize();
            float tileSize = _grids[index].GetGridTileSize();

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            float depthOffset = 6f;
            float sideSizeOffset = (gridSize.x * tileSize);
            go.transform.localScale = new Vector3(sideSizeOffset, 1f, depthOffset);
            go.transform.position = startingGridPosition + new Vector3((gridSize.x * tileSize) / 2f, 0f, depthOffset + (gridSize.y * tileSize));
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
        if (prefabPool.Length > 0){
            foreach (GameObject item in prefabPool){
                float scaleSize = (item.transform.lossyScale.x + item.transform.lossyScale.z) / 2;
                averageOffset += scaleSize;
            }
            averageOffset /= prefabPool.Length;
        }
        return averageOffset;
    }
}