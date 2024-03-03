using System;
using System.Threading.Tasks;
using UnityEngine;
using URandom = UnityEngine.Random;

public class TileGrid{
    public event EventHandler<OnPositionChangedEventArgs> OnPositionChanged;
    public event EventHandler OnGridDestroying;

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
    private bool _creationSuspended = false;

    public TileGrid(int xSize, int ySize , float offset, bool isSmooth, GameObject[] prefabs, Vector3 startingPosition = default) {
        if (xSize <= 0 || ySize <= 0) return;
        if (prefabs == null || prefabs.Length == 0) return;
        _width = xSize;
        _height = ySize;
        _gridArray = new GameObject[xSize, ySize];
        _tileOffset = offset;
        _startingPosition = startingPosition;
        CreateGrid(prefabs, isSmooth);
    }

    public TileGrid(float offset, bool isSmooth, GameObject[,] mapping, Vector3 startingPosition = default)
    {
        if(mapping.GetLength(0) <=0 || mapping.GetLength(1) <=0) return;
        _width = mapping.GetLength(0);
        _height = mapping.GetLength(1);
        _gridArray = new GameObject[_width, _height];
        _tileOffset = offset;
        _startingPosition = startingPosition;
        CreateGrid(mapping, isSmooth);
    }

    ~TileGrid(){
        Debug.Log("DESTRUCTOR FOR TILEGRID WAS CALLED");
        _gridArray = null;
        _creationSuspended = true;
    }

    private async void CreateGrid(GameObject[] prefabs, bool isSmooth = false){
        int seed = URandom.Range(-9999, 9999);
        URandom.InitState(seed);
        for (int y = 0; y < _height; y++){
            for (int x = 0; x < _width; x++){
                if (_creationSuspended) return;
                URandom.State newState = URandom.state;
                int randIndex = URandom.Range(0, prefabs.Length);
                GameObject prefab = null;
                if (randIndex < prefabs.Length) prefab = prefabs[randIndex];
                CreateGridElement(prefab, x, y);
                MoveGridElementToPosition(x, y, isSmooth);
                await Task.Delay(1000/(_height * _width));
            }
        }
    }

    private async void CreateGrid(GameObject[,] mapping, bool isSmooth = false){
        for (int x = 0; x < _width; x++){
            for (int y = 0; y < _height; y++){
                if (_creationSuspended) return;
                CreateGridElement(mapping[x,y], x, y);
                MoveGridElementToPosition(x, y, isSmooth);
                await Task.Delay(1000 / (_height * _width));
            }
        }
    }

    private async void CreateGridElement(GameObject selected, int x, int y){
        GameObject element = null;
        if(selected != null) {
            element = UnityEngine.Object.Instantiate(selected, _startingPosition + Vector3.down * 10f, Quaternion.identity);
            element.name = selected.name+"("+ x +"_"+ y +")";
            element.GetComponent<Tile>().AssignTileGrid(this);
            element.GetComponent<TileMove>().AssignTileGrid(this);
        }
        _gridArray[x, y] = element;
        await Task.Delay(100 / (_height * _width));
    }

    //public bool GridElementExists(int x, int y) => (_gridArray[x, y] is not null) ? true : false;

    //public Vector2Int GetTileIndices(GameObject element){
    //    for(int x= 0; x < _width; x++){
    //        for(int y=0; y < _height; y++){
    //            if (_gridArray[x, y] != null && _gridArray[x, y] == element) return new Vector2Int(x, y);
    //        }
    //    }
    //    return new Vector2Int(-1, -1);
    //}

    public void UpdateGridOffset(float offset, bool isSmoothChange = false){
        _tileOffset = offset;
        for (int x = 0; x < _width; x++){
            for (int y = 0; y < _height; y++){
                MoveGridElementToPosition(x, y, isSmoothChange);
            }
        }
    }

    public void UpdateStartingPosition(Vector3 newStartingPosition) => _startingPosition = newStartingPosition;

    //public Vector2 GetGridSize() => new Vector2(_width, _height);

    //public float GetGridTileSize(){
    //    foreach(var element in _gridArray){
    //        if(element == null) continue;
    //        else return (element.transform.lossyScale.x + element.transform.lossyScale.z) / 2f;
    //    }
    //    return 1f;
    //}

    public Vector3 GetUpperRightCornerPosition() => _startingPosition + new Vector3((_width - 1) * _tileOffset, 0f, (_height - 1) * _tileOffset);

    public Vector3 GetUpperLeftCornerPosition() => _startingPosition + new Vector3(0f * _tileOffset, 0f, (_height - 1) * _tileOffset);

    public void DestroyGridElements() => OnGridDestroying?.Invoke(this, EventArgs.Empty);

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