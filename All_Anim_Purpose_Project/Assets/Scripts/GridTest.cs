using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTest : MonoBehaviour{
    private MyGrid grid;
    [Range(1f, 10f)]
    [SerializeField] private float offset = 1f;
    [SerializeField] private GameObject prefab;

    private void Start(){
        grid = new MyGrid(5, 5, offset, true, prefab);
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.R)) {
            grid.UpdateGridOffset(offset, true);
        }

        if (Input.GetKeyDown(KeyCode.F)){
            CursorController.Instance.CheckRaycast();
        }
    }
}
