using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility.PlaceUtility;

public class DroppableCollector : MonoBehaviour{
    [SerializeField] List<LayerMask> _removeablesLayMaskList;
    [SerializeField] List<LayerMask> _respawnablesLayerMaskList;
    private int buildIndex;

    private void Start(){
        buildIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void OnCollisionEnter(Collision collision){
        LayerMask mask = LayerUtility.GetLayerMask(collision.gameObject);

        //Respawn phase (where applicable)
        if (_respawnablesLayerMaskList.Contains(mask)){
            //Go through all respawn scenarios.
            if (PlaceLoadingUtility.IsPlace(buildIndex, PlaceLoadingUtility.Place.ObstacleCourse)) MyGameManager.Instance.RequestRespawnPlayer();
            else if (PlaceLoadingUtility.IsPlace(buildIndex, PlaceLoadingUtility.Place.LevelCreator)) MyGameManager.Instance.SimplePlayerRespawn();
            else MyGameManager.Instance.SimplePlayerRespawn();
            return;
        }

        //Destroy phase (where applicable)
        if (_removeablesLayMaskList.Contains(mask)) {
            Destroy(collision.transform.root.gameObject);
            return;
        }
    }
}