using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppableCollector : MonoBehaviour{
    [SerializeField]List<LayerMask> _preventDestroyList;
    private void OnCollisionEnter(Collision collision){
        LayerMask mask = LayerUtility.GetLayerMask(collision.gameObject);
        
        if (_preventDestroyList.Contains(mask)) Destroy(collision.transform.root.gameObject);
    }
}