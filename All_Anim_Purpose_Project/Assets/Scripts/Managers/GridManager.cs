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
    private float baseOffset = 4.5f;
    private int _checkPointReached = -1;
    private int _finalGridCount = -1;
    [SerializeField][Range(2,40)] private int testGridSizeX,testGridSizeY;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateGrid(testGridSizeX, testGridSizeY, true);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpdateGrid(0);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            DestroyGrid(0);
        }
    }

    //Event Hooks
    private void SceneManager_OnSceneLoaded(Scene arg0, LoadSceneMode arg1){
        StartCoroutine(WaitForLoading(1.5f));
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
        List<GameObject> pool = new List<GameObject>();
        foreach (GridPoolObjectSO gridPoolObjectSO in gridPoolObjectList) pool.Add(gridPoolObjectSO.PoolObject);

        TileGrid tileGrid = new TileGrid(sizeX, sizeY, baseOffset + extraOffset, true, pool.ToArray(), startingGridPosition);
        _grids.Add(tileGrid);
        if(hasCheckPoint) CreateGridCheckpoint(_grids.Count-1); 
    }

    public void CreatePredefinedGrid(GameObject[,] userDefinedTileMapping, bool hasCheckPoint = true){
        TileGrid tileGrid = new TileGrid(baseOffset + extraOffset, true, userDefinedTileMapping, startingGridPosition);
        _grids.Add(tileGrid);
        if (hasCheckPoint) CreateGridCheckpoint(_grids.Count - 1);
    }


    //TODO FIX
    private void CreateGridCheckpoint(int index = -1){
        StartCoroutine(WaitForCheckPointCreation(3f, index));
    }

    private void UpdateGrid(int index = -1){
        if (_grids.Count > 0 && _grids[index] != null){
            _grids[index].UpdateStartingPosition(startingGridPosition);
            _grids[index].UpdateGridOffset(baseOffset + extraOffset, true);
        }
    }

    IEnumerator WaitForLoading(float duration){
        Debug.Log("Waiting for loading for Place: " + GetCurrentPlace().ToString());
        yield return new WaitForSeconds(duration);
        switch (GetCurrentPlace())
        {
            case Place.ObstacleCourse:
                //Difficulty Based (Random)
                DifficultyPresetSO difficultyPresetSO = LevelUtility.GetActiveDifficultyPreset();
                if (difficultyPresetSO != null)
                {
                    _finalGridCount = difficultyPresetSO.GetGridCount(); //The length of the list (to have).
                    gridPoolObjectList = difficultyPresetSO.GetGridPoolObjectSOList().ToArray();
                    if (gridPoolObjectList.Length == 0) yield return null;
                    CreateGrid(difficultyPresetSO.GetGridSizeX(), difficultyPresetSO.GetGridSizeY());
                    yield return null;
                }
                //User Defined Difficulty Custom Template
                UserDefinedMappedDifficultySO userDefinedMappedDifficultySO = LevelUtility.GetActiveUserDefinedMappedDifficulty();
                if (userDefinedMappedDifficultySO != null)
                {
                    _finalGridCount = userDefinedMappedDifficultySO.GetTemplateGridCount();
                    GameObject[,] mapping = userDefinedMappedDifficultySO.GetConvertedTileMapToGameObjects();
                    CreatePredefinedGrid(mapping);
                    yield return null;
                }
                break;
        }
    }

    IEnumerator WaitForCheckPointCreation(float duration, int index)
    {
        yield return new WaitForSeconds(duration);
        if (_grids[index] is not null){
            //Variables
            Vector3 upperLeftCorner = _grids[index].GetUpperLeftCornerPosition();
            Vector3 upperRightCorner = _grids[index].GetUpperRightCornerPosition();
            
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube); //Checkpoint primitive.

            //Scale (Checkpoint)
            float checkpointXAxisScale = Mathf.Abs(upperLeftCorner.x - upperRightCorner.x);
            float checkpointZAxisScale = 6f;
            go.transform.localScale = new Vector3(checkpointXAxisScale, 1f, checkpointZAxisScale);

            //Position (Checkpoint)
            float xPos = (upperRightCorner.x + upperLeftCorner.x)/2f;
            float zPos = upperLeftCorner.z + baseOffset + extraOffset;
            go.transform.position = new Vector3(xPos, 0f, zPos);

            //Setup Checkpoint Component on Creation (with parameters)
            CheckPoint checkpoint = go.AddComponent<CheckPoint>();
            checkpoint.SetCheckPointIndex(_checkPointReached + 1);
        }
    }
}