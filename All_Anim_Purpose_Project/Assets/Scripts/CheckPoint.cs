using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider))]
public class CheckPoint : MonoBehaviour{
    public static event EventHandler<OnCheckPointReachedEventArgs> OnCheckPointReached;
    private bool hasReached = false;

    public class OnCheckPointReachedEventArgs : EventArgs{
        public int index;
        public Transform self;
    }

    private string[] layerNames = { "Player" };
    private int checkpointIndex = -1;

    private void OnCollisionEnter(Collision collision){
        if (LayerUtility.LayerIsName(collision.gameObject.layer, layerNames)){
            if (!hasReached){
                MyGameManager.Instance.ChangeRespawnPoint(transform);
                OnCheckPointReached?.Invoke(this, new OnCheckPointReachedEventArgs{
                    index = checkpointIndex,
                    self = transform,
                });
                hasReached = true;
            }
        }
    }

    public void SetCheckPointIndex(int index) => checkpointIndex = index;
}
