using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using URandom = UnityEngine.Random;

public class DroppableSpawnManager : Singleton<DroppableSpawnManager>{
    [SerializeField] private GridPoolObjectSO[] droppables;
    [SerializeField] private Transform spawnPoint;

    private void Update(){
        if (Input.GetKeyDown(KeyCode.E)) SpawnDroppable();
    }

    private void SpawnDroppable(){        
        if (droppables.Length > 0){
            //Seed
            int seed = URandom.Range(-9999, 9999);
            URandom.InitState(seed);
            
            //Get Raycast Position Marked
            GameObject hit = MouseUtility.GetMouseToWorldRayHit();
            
            //Random Sample Droppable
            URandom.State newState = URandom.state;
            int index = URandom.Range(0, droppables.Length);
            Instantiate(droppables[index].PoolObject, GetSpawnPosition(hit), Quaternion.identity);
        }
    }

    private Vector3 GetRandomOffsetFromRange(float min,  float max) => new Vector3(URandom.Range(min, max), 0f, URandom.Range(min, max));
   
    private Vector3 GetSpawnPosition(GameObject hit){
        Vector3 spawnPosition;
        Vector3 randomOffset = GetRandomOffsetFromRange(0f, 2f);
        if (hit != null) spawnPosition = new Vector3(hit.transform.position.x, spawnPoint.position.y, hit.transform.position.z) + randomOffset;
        else spawnPosition = spawnPoint.position + randomOffset;

        return spawnPosition;
    }
}
