using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPrompt : MonoBehaviour{
    private string[] layerNames = {"Player"};
    [SerializeField] PlaceLoadingUtility.Place targetPlace;
    private bool teleportStarted = false;
    private Rigidbody targetRb;
    private GameObject targetGo;
    private float teleportTime = 4f;
    private float teleportTimeElapsed = 0f;
    private float eulerRotationYAxisRatio = 15f;

    private void OnTriggerEnter(Collider other){
        if (LayerUtility.LayerIsName(other.gameObject.layer, layerNames))
        {
            if (!teleportStarted){
                Debug.Log("Teleport starting in 4... Please wait!");
                InputManager.Instance.SetControlLockStatus(true);
                targetRb = other.gameObject.GetComponent<Rigidbody>();
                targetGo = other.gameObject;
                teleportStarted = true;
                StartCoroutine(WaitForTeleport());
            }
        }
    }

    private void OnTriggerExit(Collider other){
        Debug.Log("Teleport Canceled by user");
        if (teleportStarted){
            InputManager.Instance.SetControlLockStatus(false);
            StopCoroutine(WaitForTeleport());
        }
    }

    private void Update(){
        if (teleportStarted) {
            teleportTimeElapsed += Time.deltaTime;
            targetRb.velocity = Vector3.zero;
            targetGo.transform.Rotate(0f, eulerRotationYAxisRatio + (eulerRotationYAxisRatio * teleportTimeElapsed), 0f);
        }
        else
        {
            if (teleportTimeElapsed != 0f) teleportTimeElapsed = 0f;
        }
        
    }

    IEnumerator WaitForTeleport(){        
        yield return new WaitForSeconds(teleportTime);
        PlaceLoadingUtility.MoveToPlace(targetPlace);
    }
}