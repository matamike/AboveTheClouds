using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Utility.PlaceUtility.PlaceLoadingUtility;

public class MyGameManager : Singleton<MyGameManager> {
    //Hooks to prefabs
    [SerializeField] GameObject inputPrefab;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject cameraPrefab;

    //1st Load Parameters
    [SerializeField] Vector3 playerSpawnPointPositionOffset;
    [SerializeField] Vector3 cameraSpawnPointOffset;
    private Transform spawnPoint;

    //Game Over Criteria.
    [SerializeField] private GameOverCriteriaSO gameOverCriteriaSO;
    private int playerTimesRespawned = 0;

    private void Start(){
        spawnPoint = GameObject.Find("SpawnPoint").transform;
        InitializeCoreComponents();
        InputManager.Instance.SetControlLockStatus(false);
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
        InputManager.Instance.SetControlLockStatus(true);
    }

    public void RequestRespawnPlayer(){
        int playerLifes = 1000; //default
        if (gameOverCriteriaSO is not null) playerLifes = gameOverCriteriaSO.GetGameOverCriteria();

        if (playerTimesRespawned < playerLifes){
            SimplePlayerRespawn();
            playerTimesRespawned++;
        }
        else{
            Debug.Log("Game Over");
            PreferencesUtility.RequestResetOneTimeForAllOfTheType(this);
            TeleportPlayerBackToHub();
        }
    }

    public void SimplePlayerRespawn() => PlayerController.Instance.transform.position = spawnPoint.transform.position + playerSpawnPointPositionOffset;

    public void TeleportPlayerBackToHub() => MoveToPlace(Place.Hub);

    public void ChangeRespawnPoint(Transform checkpoint) => spawnPoint = checkpoint;
}
