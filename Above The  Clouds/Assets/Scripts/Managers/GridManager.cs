using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Utility.PlaceUtility.PlaceLoadingUtility;

public class GridManager : Singleton<GridManager>{
    public event EventHandler<EventArgs> OnLastCheckpointReached;

    [Range(0f, 10f)] private float extraOffset = 1f;
    [SerializeField] private GridPoolObjectSO[] gridPoolObjectList;
    [SerializeField] private Vector3 startingGridPosition;
    [SerializeField] private List<TileGrid> _grids = new List<TileGrid>();
    private float baseOffset = 4.5f;
    private int _checkPointReached = -1;
    private int _finalGridCount = -1;
    [SerializeField][Range(2,400)] private int testGridSizeX,testGridSizeY;
    [SerializeField] private GameObject checkpointPrefab;

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
        if (Input.GetKeyDown(KeyCode.E)) CreateGrid(testGridSizeX, testGridSizeY);
        if (Input.GetKeyDown(KeyCode.R)) DestroyGrid(0);
    }

    //Event Hooks
    private void SceneManager_OnSceneLoaded(Scene arg0, LoadSceneMode arg1){
        StartCoroutine(WaitForLoading(1.5f));
    }

    private void CheckPoint_OnCheckPointReached(object sender, CheckPoint.OnCheckPointReachedEventArgs e) {
        startingGridPosition = e.self.position + new Vector3(0,0, baseOffset + extraOffset);
        _checkPointReached = e.index;
        DestroyGrid(0);
        if (_checkPointReached < (_finalGridCount - 1)) {
            DifficultyPresetSO difficultyPresetSO = LevelUtility.GetActiveDifficultyPreset();
            gridPoolObjectList = difficultyPresetSO.GetGridPoolObjectSOList().ToArray();
            CreateGrid(difficultyPresetSO.GetGridSizeX(), difficultyPresetSO.GetGridSizeY());
        }
        else if(_checkPointReached == (_finalGridCount - 1)){
            DestroyAllGrids();
            OnLastCheckpointReached?.Invoke(this, EventArgs.Empty);
        }
    }
    /////////////
    
    //Member Functions
    private void DestroyAllGrids(){
        if (_grids.Count == 0) return;
        for(int i = _grids.Count - 1; i >= 0; i--) DestroyGrid(i);
    }
    private void DestroyGrid(int gridIndex){
        if (_grids.Count > 0 && _grids.ElementAt(gridIndex) != null){
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
        if(hasCheckPoint) CreateCheckPoint(_grids.Count-1); 
    }

    public void CreatePredefinedGrid(GameObject[,] userDefinedTileMapping, bool hasCheckPoint = true){
        TileGrid tileGrid = new TileGrid(baseOffset + extraOffset, true, userDefinedTileMapping, startingGridPosition);
        _grids.Add(tileGrid);
        if (hasCheckPoint) CreateCheckPoint(_grids.Count - 1);
    }

    private void UpdateGrid(int index = -1){
        if (_grids.Count > 0 && _grids.ElementAt(index) != null){
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
                if (difficultyPresetSO != null){
                    _finalGridCount = difficultyPresetSO.GetGridCount(); //The length of the list (to have).
                    gridPoolObjectList = difficultyPresetSO.GetGridPoolObjectSOList().ToArray();
                    if (gridPoolObjectList.Length == 0) yield return null;
                    CreateGrid(difficultyPresetSO.GetGridSizeX(), difficultyPresetSO.GetGridSizeY());
                    yield return null;
                }
                //User Defined Difficulty Custom Template
                UserDefinedMappedDifficultySO userDefinedMappedDifficultySO = LevelUtility.GetActiveUserDefinedMappedDifficulty();
                if (userDefinedMappedDifficultySO != null){
                    _finalGridCount = userDefinedMappedDifficultySO.GetTemplateGridCount();
                    GameObject[,] mapping = userDefinedMappedDifficultySO.GetConvertedTileMapToGameObjects();
                    CreatePredefinedGrid(mapping);
                    yield return null;
                }
                break;
        }
    }

    private void CreateCheckPoint(int index = -1){
        if (_grids.ElementAt(index) == null) return;
        else{
            //Variables
            Vector3 upperLeftCorner = _grids[index].GetUpperLeftCornerPosition();
            Vector3 upperRightCorner = _grids[index].GetUpperRightCornerPosition();

            GameObject go = Instantiate(checkpointPrefab);

            //Scale (Checkpoint)
            float checkpointXAxisScale = Mathf.Abs(upperLeftCorner.x - upperRightCorner.x);
            float checkpointZAxisScale = 6f;
            go.transform.localScale = new Vector3(checkpointXAxisScale, 1f, checkpointZAxisScale);

            //Position (Checkpoint)
            float xPos = (upperRightCorner.x + upperLeftCorner.x) / 2f;
            float zPos = upperLeftCorner.z + baseOffset + extraOffset;
            go.transform.position = new Vector3(xPos, 0f, zPos);

            //Setup Checkpoint Component on Creation (with parameters)
            CheckPoint checkpoint = go.GetComponent<CheckPoint>();
            checkpoint.SetCheckPointIndex(_checkPointReached + 1);
            //Create bond between checkpoint and grid
            checkpoint.SetElementBoundToCheckoint(_grids[index]); 
        }
    }
}