using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private bool isRotating = false;
    private float rotateDuration = 2f;
    private float rotateCooldown = 1f;
    private float rotateTimeElapsed;
    private bool isInCooldown = false;

    //private List<GameObject> interactingGameObjects = new List<GameObject>();

    private void Update(){
        RotatingProcess();//Rotating
        Cooldown(); //Cooldown
    }

    private void OnCollisionStay(Collision collision){
        if (collision.gameObject.TryGetComponent(out DropSource dropSource)){
            //TryAddingInteractingSource(dropSource.gameObject);
            isRotating = true;
        }
    }

    private void OnCollisionExit(Collision collision){
        if (collision.gameObject.TryGetComponent(out DropSource dropSource)){
            //TryRemoveInteractingSource(dropSource.gameObject);
            isRotating = false;
        }
    }

    private void RotatingProcess(){
        if (isRotating && !isInCooldown){
            gameObject.transform.Rotate(Vector3.up, Random.Range(-1, 1f) * 300f);
            rotateTimeElapsed += Time.deltaTime;
            //Duration Exceeded
            if (rotateTimeElapsed > rotateDuration){
                isRotating = false;
                isInCooldown = true;
            }
        }
        else gameObject.transform.eulerAngles = Vector3.Lerp(gameObject.transform.transform.eulerAngles, Vector3.zero, 5f * Time.deltaTime);
    }

    private void Cooldown(){
        if (isInCooldown){
            rotateTimeElapsed += Time.deltaTime;
            if (rotateTimeElapsed > (rotateDuration + rotateCooldown)) isInCooldown = false;
        }
    }

    //private void TryAddingInteractingSource(GameObject droppableSourceGO){
        //if (!interactingGameObjects.Contains(droppableSourceGO)) interactingGameObjects.Add(droppableSourceGO);
    //}

    //private void TryRemoveInteractingSource(GameObject droppableSourceGO){
        //if (interactingGameObjects.Contains(droppableSourceGO)) interactingGameObjects.Remove(droppableSourceGO);
    //}
}