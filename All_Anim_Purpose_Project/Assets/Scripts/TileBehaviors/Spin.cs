using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SearchService;
using UnityEngine;

public class Spin : MonoBehaviour, IInteractable{
    [SerializeField] private bool isRotating = false;
    [SerializeField] private bool isInCooldown = false;
    [SerializeField] private float rotateDuration = 0.0f,rotateCooldown = 0.0f;
    [SerializeField] private float rotateTimeElapsed = 0.0f;
    [SerializeField] [Range(1f,10f)]private float speed = 1f;

    private Rigidbody playerRB;
    private string[] layerNames = { "Player", "DroppedObject" };


    private void Update(){
        if (!isInCooldown) RotatingProcess();//Rotating
        else Cooldown(); //Cooldown
    }

    private void RotatingProcess(){
        if (isRotating){
            //Transform Rotation
            gameObject.transform.root.transform.eulerAngles += new Vector3(0, rotateDuration * rotateTimeElapsed, 0) * speed;

            //Apply force to player Rigidbody
            ApplyForceToTarget();

            // Progression of time.
            rotateTimeElapsed += Time.deltaTime;

            //Duration Exceeded
            if (rotateTimeElapsed > rotateDuration){
                isRotating = false;
                isInCooldown = true;
                rotateTimeElapsed = 0.0f;
            }
        }
    }

    private void Cooldown(){
        if (isInCooldown){
            gameObject.transform.eulerAngles = Vector3.Lerp(gameObject.transform.transform.eulerAngles, Vector3.zero, 1f);
            rotateTimeElapsed += Time.deltaTime;
            if (rotateTimeElapsed > rotateCooldown){
                isInCooldown = false;
                rotateTimeElapsed = 0.0f;
            }     
        }  
    }

    private void ApplyForceToTarget(){
        speed = Mathf.Lerp(speed, 10f, speed * Time.deltaTime);
        float velocityXAxis = Mathf.Sin(2f * Mathf.PI * rotateTimeElapsed * Time.deltaTime) * speed;
        float velocityZAxis = Mathf.Sin(2f * Mathf.PI * rotateTimeElapsed * Time.deltaTime) * speed;
        Vector3 targetVelocity = new Vector3(velocityXAxis, 0f, velocityZAxis);
        if (playerRB != null){
            playerRB.velocity = targetVelocity;
            playerRB.transform.eulerAngles = transform.eulerAngles;
        }
    }

    //IInteractable Interface
    public void Interact(GameObject invokeSource){
        if (LayerUtility.LayerIsName(invokeSource.layer, layerNames)){
            if (!isInCooldown){
                playerRB = invokeSource.GetComponent<Rigidbody>();
                isRotating = true;
            }
        }
    }

    public void CancelInteracion(GameObject invokeSource){
        if (playerRB != null) playerRB.AddForce(Vector3.up * speed, ForceMode.Impulse);
        playerRB = null;
    }
}