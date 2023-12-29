using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility.PlaceUtility.PlaceLoadingUtility;

public class TeleportPrompt : MonoBehaviour{
    private string[] layerNames = {"Player"};
    [SerializeField] Place targetPlace;
    [SerializeField] DifficultyPresetSO _targetDifficultyPresetSO;
    [SerializeField] UserDefinedMappedDifficultySO _userDefinedMappedDifficultySO;
    [SerializeField] bool requireCustomDifficultyUIPrompt = false;
    
    //Player Components Ref
    private Rigidbody targetRb;
    private GameObject targetGo;

    //Teleporting Process parameters
    private bool teleportStarted = false;
    private float teleportTime = 4f;
    private float teleportTimeElapsed = 0f;
    private float eulerRotationYAxisRatio = 15f;

    private void Start(){
        if (UserDefinedTemplateUIController.Instance != null){
            UserDefinedTemplateUIController.Instance.OnCustomDifficultySelected += UserDefinedTemplateUIController_OnCustomDifficultySelected;
        }
    }

    private void OnDisable(){
        if (UserDefinedTemplateUIController.Instance != null){
            UserDefinedTemplateUIController.Instance.OnCustomDifficultySelected -= UserDefinedTemplateUIController_OnCustomDifficultySelected;
        }
    }

    private void UserDefinedTemplateUIController_OnCustomDifficultySelected(object sender, UserDefinedTemplateUIController.OnCustomDifficultySelectedEventArgs e){
        _userDefinedMappedDifficultySO = e.userDefinedMappedDifficultySO;
        StartTransition(targetGo);
    }


    private void OnTriggerEnter(Collider other){
        if (LayerUtility.LayerIsName(other.gameObject.layer, layerNames)){

            //Only for User Defined Difficulty Portal
            if (requireCustomDifficultyUIPrompt){
                UserDefinedTemplateUIController.Instance.ToggleCanvas();
                return;
            }

            StartTransition(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other){
        //Only for User Defined Difficulty Portal
        if (requireCustomDifficultyUIPrompt){
            UserDefinedTemplateUIController.Instance.ToggleCanvas();
            return;
        }

        StopTransition();
    }

    private void Update(){
        if (teleportStarted) {
            teleportTimeElapsed += Time.deltaTime;
            if (targetGo != null){
                targetRb.velocity = Vector3.zero;
                targetGo.transform.Rotate(0f, eulerRotationYAxisRatio + (eulerRotationYAxisRatio * teleportTimeElapsed), 0f);
            }
        }
        else{
            if (teleportTimeElapsed != 0f) teleportTimeElapsed = 0f;
        }
    }

    private void StartTransition(GameObject go){
        if (!teleportStarted){
            //Prepare LevelUtility before teleporting to handle the mode difficulty.
            if (_targetDifficultyPresetSO is not null) LevelUtility.SetDifficultyModeWithRandomPlacement(_targetDifficultyPresetSO);
            else if (_userDefinedMappedDifficultySO is not null) LevelUtility.SetUserDefinedMappedDifficulty(_userDefinedMappedDifficultySO);
            else{
                if (targetPlace != Place.LevelCreator) return;
            }

            //Prepare for Teleport
            InputManager.Instance.SetControlLockStatus(true);
            if (targetGo != null){
                targetRb = go.gameObject.GetComponent<Rigidbody>();
                targetGo = go.gameObject;
            }
            teleportStarted = true;
            StartCoroutine(WaitForTeleport());
        }
    }

    private void StopTransition(){
        if (teleportStarted){
            InputManager.Instance.SetControlLockStatus(false);
            teleportStarted = false;
            StopCoroutine(WaitForTeleport());
        }
    }

    IEnumerator WaitForTeleport(){        
        yield return new WaitForSeconds(teleportTime);
        if(teleportStarted) MoveToPlace(targetPlace);
    }
}