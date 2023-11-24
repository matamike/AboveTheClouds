using System.Collections;
using UnityEngine;

public class MyGameManager : Singleton<MyGameManager> {
    [SerializeField] GameObject inputPrefab;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject cameraPrefab;

    [SerializeField] Vector3 playerSpawnPointPositionOffset;
    [SerializeField] Vector3 cameraSpawnPointOffset;
    private Transform spawnPoint;
    private int playerTimesRespawned = 0;
    private int maxPlayerTimesRespawn = 3;

    private void Start(){
        spawnPoint = GameObject.Find("SpawnPoint").transform;
        InitializeCoreComponents();
        InitializePlaceComponents();
        TogglePlayerControlsLocked(false);
    }

    private void InitializeCoreComponents(){
        //Generate Input System profile (events to keys/callbacks etc.)
        Instantiate(inputPrefab, Vector3.zero, Quaternion.identity); 
        //Generate Player
        GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.transform.position + playerSpawnPointPositionOffset,Quaternion.identity); 
        //Generate/Setup Camera
        Instantiate(cameraPrefab, spawnPoint.transform.position + cameraSpawnPointOffset, Quaternion.identity);
        Transform cameraFocusTransform = playerInstance.transform.Find("Focus"); 
        CameraController.Instance.AssignFollowTransform(cameraFocusTransform); 
        CameraController.Instance.AssignRotateAroundTransform(cameraFocusTransform);
        TogglePlayerControlsLocked(true);
    }

    private void InitializePlaceComponents(){
        PlaceLoadingUtility.Place place = PlaceLoadingUtility.GetCurrentPlace();

        switch (place){
            case PlaceLoadingUtility.Place.None:
                Debug.Log("Place loaded: (None)");
                break;
            case PlaceLoadingUtility.Place.Hub:
                Debug.Log("Place loaded: (Hub)");
                break;
            case PlaceLoadingUtility.Place.ObstacleCourseRandom_Any:
                Debug.Log("Place loaded: (ObstacleCourseRandom_Any)");
                GridManager.Instance.CreateGrid(); //Generate a tilemap (RandomSize)
                break;
            case PlaceLoadingUtility.Place.ObstacleCourseRandom_Easy:
                Debug.Log("Place loaded: (ObstacleCourseRandom_Easy)");
                GridManager.Instance.CreateGrid(4, 4); //Generate a tilemap (RandomSize)
                break;
            case PlaceLoadingUtility.Place.ObstacleCourseRandom_Medium:
                Debug.Log("Place loaded: (ObstacleCourseRandom_Medium)");
                GridManager.Instance.CreateGrid(8, 8); //Generate a tilemap (RandomSize)
                break;
            case PlaceLoadingUtility.Place.ObstacleCourseRandom_Hard:
                Debug.Log("Place loaded: (ObstacleCourseRandom_Hard)");
                GridManager.Instance.CreateGrid(10, 10); //Generate a tilemap (RandomSize)
                break;
        }
        
    }

    private void TogglePlayerControlsLocked(bool flag){
        InputManager.Instance.SetControlLockStatus(flag);
    }

    public void RequestRespawnPlayer(){
        //we need a way to decide the spawn point (when in hub) / (when in game) etc. -> We need to retrieve it (in case of game the starting is always the same)
        //once we reach a certain checkpoint we need to alter the spawnpoint inside the level accordingly.
        if (playerTimesRespawned < maxPlayerTimesRespawn){
            PlayerController.Instance.transform.position = spawnPoint.transform.position + playerSpawnPointPositionOffset;
            playerTimesRespawned++;
        }
        else
        {
            Debug.Log("Game Over");
            PlaceLoadingUtility.MoveToPlace(PlaceLoadingUtility.Place.Hub);
        }
    }

    public void ChangeRespawnPoint(Transform checkpoint) => spawnPoint = checkpoint;

    //IEnumerator WaitTime()
    //{
    //    Debug.Log("Wait for Core Modules to load");
    //    yield return new WaitForSeconds(4f);
    //    Debug.Log("Proceeding");
    //}
}
