using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour{
    private bool isMoving = false;
    private bool isRotating = false;
    private Vector3 targetPosition;


    [SerializeField] [Range(1f,10f)]private float duration = 5f;
    private float timer = 0f;


    private void CursorController_OnObjectFocusLost(object sender, CursorController.OnObjectFocusEventArgs e)
    {
        isRotating = false;
    }

    private void CursorController_OnObjectFocusGained(object sender, CursorController.OnObjectFocusEventArgs e)
    {
        if (e.focus == gameObject) isRotating = true;
        else isRotating = false;
    }

    private void Awake(){
        MyGrid.OnPositionChanged += MyGrid_OnPositionChanged;
        CursorController.Instance.OnObjectFocusGained += CursorController_OnObjectFocusGained;
        CursorController.Instance.OnObjectFocusLost += CursorController_OnObjectFocusLost;
        targetPosition = transform.position;
    }

    private void MyGrid_OnPositionChanged(object sender, MyGrid.OnPositionChangedEventArgs e){
        if (e.self == gameObject)
        {
            isMoving = true;
            targetPosition = e.target;
        }
    }

    private void Update()
    {
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
            gameObject.transform.Rotate(Vector3.up, 45f * Time.deltaTime);
        }
        else
        {
            gameObject.transform.eulerAngles = Vector3.Lerp(gameObject.transform.transform.eulerAngles, Vector3.zero, 10f * Time.deltaTime);
        }
        
    }
}
