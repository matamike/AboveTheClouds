using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DifficultyPresetSO : ScriptableObject{
    [Tooltip("Define the Prefab Pool of Gameobjects for this Difficulty")]
    [SerializeField] private List<GameObject> tilePool = new List<GameObject>();
    [Tooltip("Size X Axis for the grid")]
    [SerializeField][Range(2, 100)] private int gridSizeX;
    [Tooltip("Size Y Axis for the grid")]
    [SerializeField][Range(2, 100)] private int gridSizeY;
    [Tooltip("Define the number of grids to be created sequentially for this difficulty preset!")]
    [SerializeField][Range(1, 10)] private int gridCount;
    
    public List<GameObject> GetTilePool() => tilePool;
    public int GetGridSizeX() => gridSizeX;
    public int GetGridSizeY() => gridSizeY;
    public int GetGridCount() => gridCount;
}