using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility.PlaceUtility;

public class UserDefinedDifficultyTemplatesController : Singleton<UserDefinedDifficultyTemplatesController> {
    [SerializeField] private List<UserDefinedMappedDifficultySO> userDefinedMappedDifficultySOs;

    //struct UserDefinedDifficultyTemplateHolder{
    //    public GameObject[,] _tempMapping;
    //    public UserDefinedDifficultyTemplateHolder(int sizeX, int sizeY) => _tempMapping = new GameObject[sizeX, sizeY];
    //    public void ModifyMapping(GameObject[,] newMapping) => _tempMapping = newMapping;
    //    public void ModifyMapping(GameObject go, int x, int y) => _tempMapping[x,y] = go;
    //    public GameObject[,] GetActiveTemplateHolderMapping() => _tempMapping;
    //}

    private int activeTemplateIndex = -1; 

    private void OnEnable(){
        SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
        LevelCreatorUIManager.Instance.OnTemplateChanged += LevelCreatorUIManager_OnTemplateChanged;
        LevelCreatorUIManager.Instance.OnTemplateRequestSave += LevelCreatorUIManager_OnTemplateRequestSave;
    }

    private void OnDisable(){
        SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
        LevelCreatorUIManager.Instance.OnTemplateChanged -= LevelCreatorUIManager_OnTemplateChanged;
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

    private void LevelCreatorUIManager_OnTemplateChanged(object sender, LevelCreatorUIManager.OnTemplateChangedEventArgs e) => activeTemplateIndex = e.templateIndex;

    //Member Functions
    private void InitializeTemplates(){
        foreach (UserDefinedMappedDifficultySO userDefinedMappedDifficultySO in userDefinedMappedDifficultySOs){
            userDefinedMappedDifficultySO.ActivateTemplate();
        }
    }
    public List<UserDefinedMappedDifficultySO> GetAllTemplates() => userDefinedMappedDifficultySOs;

    //public  void LoadTemplate(int index) => SetActiveTemplate(CorrectIndex(index));

    //public void SaveChangesToTemplate(){
    //    GameObject[,] latestUpdatedMapping = _activeMappedDifficultyContainer.GetActiveTemplateHolderMapping();
    //    userDefinedMappedDifficultySOs[activeTemplateIndex].SaveTemplateMapping(latestUpdatedMapping);
    //}

    //public void UpdateActiveTemplate(GameObject[,] tileMapping) => _activeMappedDifficultyContainer.ModifyMapping(tileMapping);

    //public GameObject[,] GetActiveTemplateMapping() => _activeMappedDifficultyContainer.GetActiveTemplateHolderMapping();

    //private void SetActiveTemplate(int index){
    //  activeTemplateIndex = CorrectIndex(index);
    //  _activeMappedDifficultyContainer.ModifyMapping(userDefinedMappedDifficultySOs[activeTemplateIndex].GetConvertedTileMapToGameObjects());
    //}

    //private int CorrectIndex(int index) {
    //    if (index >= userDefinedMappedDifficultySOs.Count) index = 0;
    //    else if (index < 0) index = userDefinedMappedDifficultySOs.Count - 1;
    //    return index;
    //}
}