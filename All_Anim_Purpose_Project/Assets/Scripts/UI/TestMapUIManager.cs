using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class TestMapUIManager : Singleton<TestMapUIManager>{
    [SerializeField] private Button exitTestButton, respawnPlayerButton, resetLevelButton, backToHubButton;
    [SerializeField] private GameObject container;
    private Action exitTestAction = null;
    private Action spawnAction = null;

    private void Start(){
        SetupSpawnAction();

        DisableTestUI();
        InitializeButtonCallbacks();
    }

    private void SetupSpawnAction(){
        spawnAction = () => {
            InputManager.Instance.SetControlLockStatus(true);
            GridManager.Instance.RequestDestroyGrid(0);
            container.SetActive(false);
            if (exitTestAction != null)
            {
                exitTestAction();
                exitTestAction = null; // remove after execution
            }
        };
    }

    private void InitializeButtonCallbacks(){
        exitTestButton.onClick.AddListener(() => { DisableTestUI(); });
        respawnPlayerButton.onClick.AddListener(() => { MyGameManager.Instance.SimplePlayerRespawn(); });
        resetLevelButton.onClick.AddListener(() => { CreateTestLevel(); });
        backToHubButton.onClick.AddListener(() => { MyGameManager.Instance.TeleportPlayerBackToHub(); });
    }

    public void CreateTestLevel(){
        GridManager.Instance.RequestDestroyGrid(0);
        UserDefinedMappedDifficultySO activeTemplate = UserDefinedDifficultyTemplatesController.Instance.GetActiveTemplate();
        GameObject[,] grid = activeTemplate.GetConvertedTileMapToGameObjects();
        GridManager.Instance.CreatePredefinedGrid(grid, false);
    }
    public void SetExitTestAction(Action action) => exitTestAction = action;
    public void EnableTestUI(){
        container.SetActive(true);
        InputManager.Instance.SetControlLockStatus(false);
    }
    public void DisableTestUI(){
        StartCoroutine(WaitForSingleton<InputManager>(spawnAction));
    }

    IEnumerator WaitForSingleton<T>(Action action = null) where T : Singleton<T> {
        yield return new WaitUntil(() => Singleton<T>.Instance != null);
        if (action != null) action();
    }
}