using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMove : MonoBehaviour{
    [SerializeField] [Range(1f,10f)]private float resetDuration;
    [SerializeField] Collider interactingCollider;
    private Vector3 targetPosition;
    private TileGrid _tileGridAssigned;
    private Action _callback;

    private void OnDisable(){
        if (_tileGridAssigned is not null) _tileGridAssigned.OnPositionChanged -= MyGrid_OnPositionChanged;
    }

    private void MyGrid_OnPositionChanged(object sender, TileGrid.OnPositionChangedEventArgs e){
        if (e.self == gameObject){
            targetPosition = (e.target != null)? e.target : transform.position;
            DisableCollider();
            _callback = EnableCollider;
            TweenParameters tween = new(gameObject, targetPosition, transform.eulerAngles, transform.localScale, 6f, 2f, _callback);
            TweenHandler.Instance.CreateTween(tween);
        }
    }

    private void DisableCollider() => interactingCollider.enabled = false;

    private void EnableCollider() => interactingCollider.enabled = true;
    public void AssignTileGrid(TileGrid tileGrid){
        _tileGridAssigned = tileGrid;
        _tileGridAssigned.OnPositionChanged += MyGrid_OnPositionChanged;
    }
}