using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeSpring : MonoBehaviour{
    [SerializeField]private SpringJoint springJoint;
    private bool charge = false;
    private bool isCharged = false;
    private bool isInCooldown = false;
    private float chargeTime = 2f;
    private float chargingTimeElapsed = 0f;
    private float cooldown = 2f;
    private float cooldownTimeElapsed = 0f;

    private void Update(){
        if (!isInCooldown){
            if (charge){
                Charge(); //Charge the Spring
                ResetChargingState(); //Fire and Reset
            }
        }
        else{
            cooldownTimeElapsed += Time.deltaTime;
            if (cooldownTimeElapsed > cooldown) isInCooldown = false;
        }
    }

    private void OnCollisionStay(Collision collision){
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Player" || LayerMask.LayerToName(collision.gameObject.layer) == "DroppedObject"){
            if (!isInCooldown) charge = true;
        }
    }

    private void OnCollisionExit(Collision collision){
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Player" || LayerMask.LayerToName(collision.gameObject.layer) == "DroppedObject"){
            ResetChargingState();
        }
    }

    private void Charge(){
        if (!isCharged){
            chargingTimeElapsed += Time.deltaTime;
            springJoint.spring = 100f;
            springJoint.tolerance = 0.75f;
            if (chargingTimeElapsed >= chargeTime){
                isCharged = true;
            }
        }
    }

    private void ResetChargingState(){
        if (isCharged){
            Debug.Log("Fire and Reset");
            isCharged = false;
            charge = false;
            chargingTimeElapsed = 0f;
            springJoint.tolerance = 0.01f;
            springJoint.spring = 100000f;
        }
    }
}