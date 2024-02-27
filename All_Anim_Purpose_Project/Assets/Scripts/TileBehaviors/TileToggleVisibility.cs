using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileToggleVisibility : MonoBehaviour{
    private MeshRenderer _meshRenderer;

    private void Awake(){
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;
    }


    private void OnTriggerEnter(Collider other){
        if (other.gameObject.name ==  "DummyMesh"){
            _meshRenderer.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other){
        if (other.gameObject.name == "DummyMesh"){
            _meshRenderer.enabled = false;
        }
    }
}
