using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour{
    [SerializeField] private bool isMoving = false;
    private Vector3 targetPosition;

    [SerializeField] [Range(1f,10f)]private float resetDuration = 5f;
    private float timer = 0f;

    private void OnEnable(){
        MyGrid.OnPositionChanged += MyGrid_OnPositionChanged;
    }

    private void OnDisable(){
        MyGrid.OnPositionChanged -= MyGrid_OnPositionChanged;
    }

    private void MyGrid_OnPositionChanged(object sender, MyGrid.OnPositionChangedEventArgs e){
        if (e.self == gameObject){
            isMoving = true;
            targetPosition = (e.target != null)? e.target : transform.position;
        }
    }

    private void Update(){
        //Moving
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