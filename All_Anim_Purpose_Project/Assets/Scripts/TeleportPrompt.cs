using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility.PlaceUtility.PlaceLoadingUtility;

public class TeleportPrompt : MonoBehaviour{
    private string[] layerNames = {"Player"};
    [SerializeField] Place targetPlace;
    [SerializeField] DifficultyPresetSO _targetDifficultyPresetSO;
    
    //Player Components Ref
    private Rigidbody targetRb;
    private GameObject targetGo;

    //Teleporting Process parameters
    private bool teleportStarted = false;
    private float teleportTime = 4f;
    private float teleportTimeElapsed = 0f;
    private float eulerRotationYAxisRatio = 15f;

    private void OnTriggerEnter(Collider other){
        if (LayerUtility.LayerIsName(other.gameObject.layer, layerNames)){
            if (!teleportStarted){
                InputManager.Instance.SetControlLockStatus(true);
                targetRb = other.gameObject.GetComponent<Rigidbody>();
                targetGo = other.gameObject;
                teleportStarted = true;
                LevelUtility.SetDifficultyModeWithRandomPlacement(_targetDifficultyPresetSO);
                StartCoroutine(WaitForTeleport());
            }
        }
    }

    private void OnTriggerExit(Collider other){
        if (teleportStarted){
            InputManager.Instance.SetControlLockStatus(false);
            //LevelUtility.SetDifficultyModeWithRandomPlacement(null);
            teleportStarted = false;
            StopCoroutine(WaitForTeleport());
        }
    }

    private void Update(){
        if (teleportStarted) {
            teleportTimeElapsed += Time.deltaTime;
            targetRb.velocity = Vector3.zero;
            targetGo.transform.Rotate(0f, eulerRotationYAxisRatio + (eulerRotationYAxisRatio * teleportTimeElapsed), 0f);
        }
        else{
            if (teleportTimeElapsed != 0f) teleportTimeElapsed = 0f;
        }
    }

    IEnumerator WaitForTeleport(){        
        yield return new WaitForSeconds(teleportTime);
        if(teleportStarted) MoveToPlace(targetPlace);
    }
}