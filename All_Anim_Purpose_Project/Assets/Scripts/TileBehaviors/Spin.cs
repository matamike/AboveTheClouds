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

    private TileAudio tileAudio;

    private void Start(){
        tileAudio = transform.root.GetComponent<TileAudio>();
    }


    private void Update(){
        if (!isInCooldown) RotatingProcess();//Rotating
        else Cooldown(); //Cooldown
    }

    private void RotatingProcess(){
        if (isRotating){
            RotateTileTransform();
            ApplyConstantForceToTarget();
            

            // Progression of time.
            rotateTimeElapsed += Time.deltaTime * TimeMultiplierUtility.GetTimeMultiplier();

            //Duration Exceeded
            if (rotateTimeElapsed > rotateDuration){
                isRotating = false;
                isInCooldown = true;
                rotateTimeElapsed = 0.0f;
                speed = 1f;
                tileAudio.StopTileSFX();
            }
            else
            {
                if (!tileAudio.IsPlaying()) tileAudio.PlayTileSFX(TileAudio.TILE_SFX_TYPE.Activation);
            }
        }
    }

    private void Cooldown(){
        if (isInCooldown){
            ResetTileTransformRotation();
            rotateTimeElapsed += Time.deltaTime * TimeMultiplierUtility.GetTimeMultiplier();
            if (rotateTimeElapsed > rotateCooldown){
                isInCooldown = false;
                rotateTimeElapsed = 0.0f;
            }     
        }  
    }

    private void RotateTileTransform() => transform.eulerAngles += new Vector3(0, rotateDuration * rotateTimeElapsed, 0) * speed * TimeMultiplierUtility.GetTimeMultiplier();
    private void ResetTileTransformRotation(){
        if(transform.eulerAngles != Vector3.zero) transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, Vector3.zero, 1f * TimeMultiplierUtility.GetTimeMultiplier());
    }
    private void ApplyConstantForceToTarget(){
        speed = Mathf.Lerp(speed, 10f, speed * Time.deltaTime * TimeMultiplierUtility.GetTimeMultiplier());
        float velocityXAxis = Mathf.Sin(2f * Mathf.PI * rotateTimeElapsed * Time.deltaTime * TimeMultiplierUtility.GetTimeMultiplier()) * speed;
        float velocityZAxis = Mathf.Sin(2f * Mathf.PI * rotateTimeElapsed * Time.deltaTime * TimeMultiplierUtility.GetTimeMultiplier()) * speed;
        Vector3 targetVelocity = new Vector3(velocityXAxis, 0f, velocityZAxis);
        if (playerRB != null){
            playerRB.velocity = targetVelocity;
            playerRB.transform.eulerAngles = transform.eulerAngles;
        }
    }
    private void ApplyInstantForceToTarget(){
        if (playerRB != null){
            Vector3 playerForward = playerRB.gameObject.transform.forward;
            Vector3 direction = new Vector3(playerForward.x, 1f, playerForward.z);
            playerRB.AddForce(direction * speed, ForceMode.Impulse);
        }
    }

    private void AssignTarget(GameObject target) => playerRB = target.GetComponent<Rigidbody>();
    private void RemoveTarget() => playerRB = null;

    //IInteractable Interface
    public void Interact(GameObject invokeSource){
        if (LayerUtility.LayerIsName(invokeSource.layer, layerNames)){
            AssignTarget(invokeSource);
            if (!isInCooldown) isRotating = true;
        }
    }

    public void CancelInteracion(GameObject invokeSource){
        if (!isInCooldown) ApplyInstantForceToTarget();
        RemoveTarget();
    }
}