using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeSpring : MonoBehaviour{
    [SerializeField] private SpringJoint springJoint;
    private string[] layerNames = { "Player", "DroppedObject" };
    private float cooldownTimeElapsed = 0f;
    private float chargingTimeElapsed = 0f;
    private bool chargingEnabled = false;
    private bool isCharged = false;
    private bool isInCooldown = false;

    //Parameters
    private float chargeTime = 2f;
    private float cooldown = 2f;

    private void Update(){
        if (!isInCooldown){
            if (chargingEnabled) Charge(); //Charge the Spring
        }
        else Cooldown();
    }

    private void OnCollisionStay(Collision collision){
        if (LayerUtility.LayerIsName(collision.gameObject.layer, layerNames)){
            if (!isInCooldown) chargingEnabled = true;
        }
    }

    private void Cooldown(){
        cooldownTimeElapsed += Time.deltaTime;
        if (cooldownTimeElapsed > cooldown) isInCooldown = false;
    }

    private void Charge(){
        if (!isCharged){
            chargingTimeElapsed += Time.deltaTime;
            springJoint.spring = Mathf.Lerp(springJoint.spring, 100f, chargingTimeElapsed * 3f);
            springJoint.tolerance = Mathf.Lerp(springJoint.tolerance, 0.75f, chargingTimeElapsed * 3f);
            
            if (chargingTimeElapsed >= chargeTime){
                isCharged = true;
                ResetSpring(); //Fire and Reset
            }
        }
    }

    private void ResetSpring(){
        chargingTimeElapsed = 0f; //reset charging time.
        isInCooldown = true; //enable cooldown before reusing
        //Fire and Reset Spring properties.
        springJoint.tolerance = 0.01f;
        springJoint.spring = 100000f;
        //Reset charging states
        isCharged = false;
        chargingEnabled = false;
        cooldownTimeElapsed = 0f;//Reset cooldown timer.
    }
}