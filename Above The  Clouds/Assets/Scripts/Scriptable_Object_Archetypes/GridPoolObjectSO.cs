using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GridPoolObjectSO : ScriptableObject{
    [SerializeField] private GameObject poolObject;
    [SerializeField] private TileType.Type tileType;

    public TileType.Type TileType { private set { } get { return tileType; } }
    public GameObject PoolObject { private set { } get { return poolObject; } }
}
