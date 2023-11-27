using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour{
    [SerializeField] private bool isMoving = false;
    [SerializeField] [Range(1f,10f)]private float resetDuration = 5f;
    private Vector3 targetPosition;
    private float timer = 0f;
    private TileGrid _tileGridAssigned;

    private void OnDisable(){
        _tileGridAssigned.OnPositionChanged -= MyGrid_OnPositionChanged;
    }

    private void MyGrid_OnPositionChanged(object sender, TileGrid.OnPositionChangedEventArgs e){
        if (e.self == gameObject){
            isMoving = true;
            targetPosition = (e.target != null)? e.target : transform.position;
        }
    }

    private void Update(){
        MoveTile();
    }

    public void AssignTileGrid(TileGrid tileGrid){
        _tileGridAssigned = tileGrid;
        _tileGridAssigned.OnPositionChanged += MyGrid_OnPositionChanged;
    }

    private void MoveTile(){
        if (isMoving && timer <= resetDuration){
            timer += Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, resetDuration * Time.deltaTime);
        }
        else{
            isMoving = false;
            timer = 0f;
        }
    }
}