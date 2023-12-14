using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility.PlaceUtility;

public class UserDefinedDifficultyTemplatesController : Singleton<UserDefinedDifficultyTemplatesController> {
    [SerializeField] private List<UserDefinedMappedDifficultySO> userDefinedMappedDifficultySOs;
    private int activeTemplateIndex = -1; 

    private void OnEnable(){
        SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
        LevelCreatorUIManager.Instance.OnTemplateChanged += LevelCreatorUIManager_OnTemplateChanged;
        LevelCreatorUIManager.Instance.OnTemplateRequestSave += LevelCreatorUIManager_OnTemplateRequestSave;
    }

    private void OnDisable(){
        SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
        LevelCreatorUIManager.Instance.OnTemplateChanged -= LevelCreatorUIManager_OnTemplateChanged;
        LevelCreatorUIManager.Instance.OnTemplateRequestSave -= LevelCreatorUIManager_OnTemplateRequestSave;
    }

    //Event Hooks
    private void SceneManager_OnSceneLoaded(Scene arg0, LoadSceneMode arg1){
        if (PlaceLoadingUtility.IsPlace(arg0.buildIndex, PlaceLoadingUtility.Place.LevelCreator)){
            InitializeTemplates();
        }
    }

    private void LevelCreatorUIManager_OnTemplateRequestSave(object sender, LevelCreatorUIManager.OnTemplateRequestSaveArgs e){
        //Get hold of the active template to apply changes
        UserDefinedMappedDifficultySO activeTemplate = userDefinedMappedDifficultySOs[activeTemplateIndex];
        if (activeTemplate == null) return;

        //Save all the changes to the SO
        foreach (var x in e.changes){
            activeTemplate.SaveNewGridTileMapValue(x.Item1.x, x.Item1.y, x.Item2);
        }
    }

    private void LevelCreatorUIManager_OnTemplateChanged(object sender, LevelCreatorUIManager.OnTemplateChangedEventArgs e){
        activeTemplateIndex = e.templateIndex;
        Debug.Log("Template Index Changed to : " + activeTemplateIndex);
    }
    //Member Functions
    private void InitializeTemplates(){
        foreach (UserDefinedMappedDifficultySO userDefinedMappedDifficultySO in userDefinedMappedDifficultySOs){
            userDefinedMappedDifficultySO.ActivateTemplate();
        }
    }
    public List<UserDefinedMappedDifficultySO> GetAllTemplates() => userDefinedMappedDifficultySOs;

    public UserDefinedMappedDifficultySO GetActiveTemplate() {
        if (activeTemplateIndex < 0 || activeTemplateIndex > (userDefinedMappedDifficultySOs.Count - 1)) return null;
        return userDefinedMappedDifficultySOs[activeTemplateIndex];
    } 
}