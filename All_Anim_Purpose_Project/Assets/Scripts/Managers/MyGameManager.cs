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
    private int playerTimesRespawned = 0;
    private int maxPlayerTimesRespawn = 3;

    private void Start(){
        spawnPoint = GameObject.Find("SpawnPoint").transform;
        InitializeCoreComponents();
        InputManager.Instance.SetControlLockStatus(false);
    }

    private void OnEnable(){
        SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
    }

    private void OnDisable(){
        SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
    }

    //Hook to SceneLoading Event
    private void SceneManager_OnSceneLoaded(Scene arg0, LoadSceneMode arg1){
        Debug.Log("GameManager: Scene Loaded -> " + arg0.name);
        // we maybe do not need to initialize player or camera in Creator scene.
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
        //we need a way to decide the spawn point (when in hub) / (when in game) etc. -> We need to retrieve it (in case of game the starting is always the same)
        //once we reach a certain checkpoint we need to alter the spawnpoint inside the level accordingly.
        if (playerTimesRespawned < maxPlayerTimesRespawn){
            PlayerController.Instance.transform.position = spawnPoint.transform.position + playerSpawnPointPositionOffset;
            playerTimesRespawned++;
        }
        else{
            Debug.Log("Game Over");
            //TODO Add a game over sequence (UI / Sound etc...)
            MoveToPlace(Place.Hub);
        }
    }

    public void ChangeRespawnPoint(Transform checkpoint) => spawnPoint = checkpoint;
}
