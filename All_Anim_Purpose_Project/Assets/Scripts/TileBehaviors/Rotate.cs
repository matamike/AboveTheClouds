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
    [SerializeField] private Vector3 forceDirection = Vector3.zero;
    //[Range(0f,100f)][SerializeField] 
    private float forceScalar = 0f;

    private void Update(){
        RotatingProcess();//Rotating
        Cooldown(); //Cooldown
    }

    private void OnCollisionStay(Collision collision){
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Player" || LayerMask.LayerToName(collision.gameObject.layer) == "DroppedObject")
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(forceDirection * forceScalar, ForceMode.Acceleration);
            isRotating = true;
        }
    }

    private void OnCollisionExit(Collision collision){
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Player" || LayerMask.LayerToName(collision.gameObject.layer) == "DroppedObject")
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            isRotating = false;
        }
    }

    private void RotatingProcess(){
        if (isRotating && !isInCooldown){
            forceScalar = Random.Range(-1, 1f) * 300f;
            gameObject.transform.Rotate(Vector3.up, forceScalar);
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