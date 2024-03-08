using System;
using UnityEngine;
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
    [SerializeField] private RespawnTimesSO gameOverCriteriaSO;
    [SerializeField] private int playerTimesRemaining = 0;
    public static EventHandler<EventArgs> OnPlayerRespawned;

    //UX Prompt Messages SOs
    [SerializeField] private UXTypeSO winUXSO;
    [SerializeField] private UXTypeSO loseUXSO;

    private void Start(){
        //Setup Game Over Criteria
        playerTimesRemaining = gameOverCriteriaSO.GetNumberOfRespawns();

        spawnPoint = GameObject.Find("SpawnPoint").transform;
        InitializeCoreComponents();
        InputManager.Instance.SetControlLockStatus(false);

        if (GridManager.Instance != null){
            GridManager.Instance.OnLastCheckpointReached += GridManager_OnLastCheckpointReached;
        }
    }

    private void OnDisable(){
        if (GridManager.Instance != null){
            GridManager.Instance.OnLastCheckpointReached -= GridManager_OnLastCheckpointReached;
        }
    }

    private void GridManager_OnLastCheckpointReached(object sender, EventArgs e) => GameWon();

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
        //Early Exit if already in game over
        if (playerTimesRemaining <= 0) return;

        //Update Remaining Spawn Times and Respawn
        playerTimesRemaining -= 1;
        SimplePlayerRespawn();

        //GAME OVER STATE
        if (playerTimesRemaining == 0) {
            GameLost();   
        }
    }

    private void GameWon(){
        UXManager.Instance.FireUX(winUXSO.GetUXDescription(), winUXSO.GetUXTitle(), null, () => { TeleportPlayerBackToHub(); });
    }

    private void GameLost(){
        UXManager.Instance.FireUX(loseUXSO.GetUXDescription(), loseUXSO.GetUXTitle(), null, () => { TeleportPlayerBackToHub(); });
    }

    public int GetRemainingLifes() => playerTimesRemaining;

    public int GetStartingLifes() => gameOverCriteriaSO.GetNumberOfRespawns();

    public void SimplePlayerRespawn(){
        OnPlayerRespawned?.Invoke(this, EventArgs.Empty);
        PlayerController.Instance.transform.position = spawnPoint.transform.position + playerSpawnPointPositionOffset;
    }
    public void TeleportPlayerBackToHub() => MoveToPlace(Place.Hub);

    public void ChangeRespawnPoint(Transform checkpoint){
        if(spawnPoint.gameObject != null) Destroy(spawnPoint.gameObject);
        spawnPoint = checkpoint;
    }
}
