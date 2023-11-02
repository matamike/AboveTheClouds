using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class DroppableSpawnerManager : MonoBehaviour{
    public DroppableSpawnerManager Instance { get; private set; }
    [SerializeField] private GameObject[] droppables;
    [SerializeField] private Transform spawnPoint;


    private void Awake(){
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) SpawnDroppable();
    }

    private void SpawnDroppable(){        
        if (droppables.Length > 0){
            //Seed
            int seed = URandom.Range(-9999, 9999);
            URandom.InitState(seed);
            
            //Get Raycast Position Marked
            GameObject hit = MouseUtility.GetMouseToWorldRayHit();

            // Offset
            Vector3 randomOffset = new Vector3(URandom.Range(0f, 2f), 0f, URandom.Range(0f, 2f));
            
            //Positioning
            Vector3 spawnPosition = Vector3.zero;
            if (hit != null) spawnPosition = new Vector3(hit.transform.position.x, spawnPoint.position.y, hit.transform.position.z) + randomOffset;
            else spawnPosition = spawnPoint.position + randomOffset; 
            
            //Random Sample Droppable
            URandom.State newState = URandom.state;
            int index = URandom.Range(0, droppables.Length);
            Instantiate(droppables[index].gameObject, spawnPosition, Quaternion.identity);
        }
        else return;
    }
}
