using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeSpring : MonoBehaviour, IInteractable{
    [SerializeField] private SpringJoint springJoint;
    //Interacting Layers
    private string[] layerNames = { "Player", "DroppedObject" };
    private TileAudio tileAudio;

    //Timers
    private float cooldownTimeElapsed = 0f;
    private float chargingTimeElapsed = 0f;
    private float restPositionTimer = 0f;

    //Conditions
    private bool isInCooldown = false;
    private bool hasObjectInteracting = false;

    //Parameters
    private float chargeTime = 5f;
    private float cooldown = 3f;

    private void Start(){
        tileAudio = transform.root.GetComponent<TileAudio>();
    }

    private void Update(){
        if (CanCharge()){
            Charge();
            if (!tileAudio.IsPlaying()) tileAudio.PlayTileSFX(TileAudio.TILE_SFX_TYPE.Activation);
            if (chargingTimeElapsed >= chargeTime) FireSpring(); //Fire Spring
        }
        else{
            if (!IsInRestPosition()) ResetSpring();
            else restPositionTimer = 0f;
        }
        Cooldown();
    }
    private bool CanCharge() => hasObjectInteracting && !isInCooldown;

    //Drops the spring for X time before releasing it.
    private void Charge(){
        chargingTimeElapsed += Time.deltaTime * TimeMultiplierUtility.GetTimeMultiplier();
        springJoint.spring = Mathf.LerpUnclamped(springJoint.spring, 100f, chargingTimeElapsed);
        springJoint.tolerance = Mathf.LerpUnclamped(springJoint.tolerance, 0.75f, chargingTimeElapsed);
    }

    private void Cooldown(){
        if (isInCooldown){
            chargingTimeElapsed = 0f;
            cooldownTimeElapsed += Time.deltaTime * TimeMultiplierUtility.GetTimeMultiplier();
            if (cooldownTimeElapsed > cooldown){
                ToggleCooldown(false);
                cooldownTimeElapsed = 0f;
            }
        }
    }

    private void ResetSpring(){
        //Fire Spring
        restPositionTimer += Time.deltaTime * TimeMultiplierUtility.GetTimeMultiplier();
        springJoint.tolerance = Mathf.Lerp(springJoint.tolerance, 0.01f, restPositionTimer);
        springJoint.spring = Mathf.Lerp(springJoint.spring, 100000f, restPositionTimer);
    }

    private void FireSpring(){
        //Fire Spring
        springJoint.tolerance = 0.01f;
        springJoint.spring = 100000f;

        //Reset Timers and activate cooldown
        ResetSpringValues();
    }

    private void ResetSpringValues(){
        // Reset Charging/Cooldown timers
        chargingTimeElapsed = 0f;
        cooldownTimeElapsed = 0f;

        //Enable Cooldown
        ToggleCooldown(true);
    }

    private void ToggleCooldown(bool flag) => isInCooldown = flag;

    private void ToggleObjectInteracting(bool flag) => hasObjectInteracting = flag;

    private bool IsInRestPosition() => (springJoint.tolerance == 0.01f && springJoint.spring == 100000f) ? true : false;

    public void Interact(GameObject invokeSource){
        if (LayerUtility.LayerIsName(invokeSource.layer, layerNames)){    
            ToggleObjectInteracting(true);
        }
    }

    public void CancelInteracion(GameObject invokeSource){
        if (LayerUtility.LayerIsName(invokeSource.layer, layerNames)){
            ToggleObjectInteracting(false);
            chargingTimeElapsed = 0f;
        }
    }
}