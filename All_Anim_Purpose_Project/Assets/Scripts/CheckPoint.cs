using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CheckPoint : MonoBehaviour{
    private string[] layerNames = { "Player" };
    private void OnCollisionStay(Collision collision){
        if (LayerUtility.LayerIsName(collision.gameObject.layer, layerNames)){
            //set checkpoint to game manager. (TODO)
            //if is final checkpoint teleport /open portal to hub.
            MyGameManager.Instance.ChangeRespawnPoint(transform);
        }
    }
}
