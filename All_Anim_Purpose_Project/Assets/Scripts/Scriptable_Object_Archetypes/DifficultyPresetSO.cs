using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DifficultyPresetSO : ScriptableObject{
    [Tooltip("Define the Prefab Pool of Gameobjects for this Difficulty")]
    [SerializeField] private List<GridPoolObjectSO> tilePool = new List<GridPoolObjectSO>();
    [Tooltip("Size X Axis for the grid")]
    [SerializeField][Range(2, 100)] private int gridSizeX;
    [Tooltip("Size Y Axis for the grid")]
    [SerializeField][Range(2, 100)] private int gridSizeY;
    [Tooltip("Define the number of grids to be created sequentially for this difficulty preset!")]
    [SerializeField][Range(1, 10)] private int gridCount;

    public List<GameObject> GetGameObjectTilePool(){
        List<GameObject> pool = new List<GameObject> ();
        foreach(GridPoolObjectSO gridPoolObjectSO in tilePool){
            pool.Add(gridPoolObjectSO.PoolObject);
        }
        return pool;
    }

    public List<GridPoolObjectSO> GetGridPoolObjectSOList() => tilePool;

    public int GetGridSizeX() => (gridSizeX <= 0) ? 2 :gridSizeX;
    public int GetGridSizeY() => (gridSizeY <= 0) ? 2 : gridSizeY;
    public int GetGridCount() => (gridCount <= 0) ? 1 : gridCount;
}