using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppableCollector : MonoBehaviour{
    [SerializeField]List<LayerMask> _removeablesLayMaskList;
    [SerializeField] List<LayerMask> _respawnablesLayerMaskList;
    private void OnCollisionEnter(Collision collision){
        LayerMask mask = LayerUtility.GetLayerMask(collision.gameObject);
        
        if (_removeablesLayMaskList.Contains(mask)) Destroy(collision.transform.root.gameObject);

        if (_respawnablesLayerMaskList.Contains(mask)) MyGameManager.Instance.RequestRespawnPlayer();
    }
}