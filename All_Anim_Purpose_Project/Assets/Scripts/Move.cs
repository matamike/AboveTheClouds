using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour{
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool isRotating = false;
    private Vector3 targetPosition;


    [SerializeField] [Range(1f,10f)]private float duration = 5f;
    private float timer = 0f;

    private void Awake(){

        targetPosition = transform.position;
    }

    private void OnEnable()
    {
        MyGrid.OnPositionChanged += MyGrid_OnPositionChanged;
        if (CursorController.Instance)
        {
            CursorController.Instance.OnObjectFocusGained += CursorController_OnObjectFocusGained;
            CursorController.Instance.OnObjectFocusLost += CursorController_OnObjectFocusLost;
        }
    }

    private void OnDisable()
    {
        MyGrid.OnPositionChanged -= MyGrid_OnPositionChanged;
        if (CursorController.Instance)
        {
            CursorController.Instance.OnObjectFocusGained -= CursorController_OnObjectFocusGained;
            CursorController.Instance.OnObjectFocusLost -= CursorController_OnObjectFocusLost;
        }
    }

    private void CursorController_OnObjectFocusLost(object sender, CursorController.OnObjectFocusEventArgs e){
        isRotating = false;
    }

    private void CursorController_OnObjectFocusGained(object sender, CursorController.OnObjectFocusEventArgs e){
        if (e.focus == gameObject) isRotating = true;
        else isRotating = false;
    }

    private void MyGrid_OnPositionChanged(object sender, MyGrid.OnPositionChangedEventArgs e){
        if (e.self == gameObject){
            isMoving = true;
            targetPosition = e.target;
        }
    }

    private void Update(){
        //Moving
        if (isMoving && timer <= duration){
            timer += Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, duration * Time.deltaTime);
        }
        else
        {
            isMoving = false;
            timer = 0f;
        }

        //Rotating
        if (isRotating)
        {
            gameObject.transform.Rotate(Vector3.up, 90f * Time.deltaTime);
        }
        else
        {
            gameObject.transform.eulerAngles = Vector3.Lerp(gameObject.transform.transform.eulerAngles, Vector3.zero, 10f * Time.deltaTime);
        }
        
    }
}
