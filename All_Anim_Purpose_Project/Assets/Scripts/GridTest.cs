using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTest : MonoBehaviour{
    [Range(1f, 10f)][SerializeField] private float offset = 1f;
    [SerializeField] private GameObject[] prefabPool;
    private TileGrid grid;

    private void Start(){
        grid = new TileGrid(15, 15, offset, true, prefabPool);
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.R)) grid.UpdateGridOffset(offset, true);
    }
}