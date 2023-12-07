using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility.PlaceUtility;

public class UserDefinedDifficultyTemplatesController : Singleton<UserDefinedDifficultyTemplatesController> {
    [SerializeField] private List<UserDefinedMappedDifficultySO> userDefinedMappedDifficultySOs;
    private int activeTemplateIndex = 0; 
    
    struct UserDefinedDifficultyTemplateHolder{
        public GameObject[,] _tempMapping;
        public UserDefinedDifficultyTemplateHolder(int sizeX, int sizeY) => _tempMapping = new GameObject[sizeX, sizeY];
        public void ModifyMapping(GameObject[,] newMapping) => _tempMapping = newMapping;
        public void ModifyMapping(GameObject go, int x, int y) => _tempMapping[x,y] = go;
        public GameObject[,] GetActiveTemplateHolderMapping() => _tempMapping;
    }

    private UserDefinedDifficultyTemplateHolder _activeMappedDifficultyContainer;

    private void Start(){
        foreach(UserDefinedMappedDifficultySO userDefinedMappedDifficultySO in userDefinedMappedDifficultySOs) userDefinedMappedDifficultySO.ActivateTemplate();
        _activeMappedDifficultyContainer = new UserDefinedDifficultyTemplateHolder(10, 10);
    }

    private void OnEnable(){
        SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
    }

    private void OnDisable(){
        SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
    }

    private void Update(){
    }

    //Event Hooks
    private void SceneManager_OnSceneLoaded(Scene arg0, LoadSceneMode arg1){
        if (PlaceLoadingUtility.IsPlace(arg0.buildIndex, PlaceLoadingUtility.Place.LevelCreator)){

            // GUI CONTROLLER SIDE LIST OF INTERACTIONS HERE.
            // The gui will handle user interactions (Load template/Save template) -> it will create a list of templates. each list will be a grid of buttons.
            // -> each button will be toggle selected. -> if selected it will prompt a list of pool object to be used. 
            // the name of each button will load/change as expected based on it's current selection.
            // with toggle on we load a selector gui underneath that will hold the pool options
            // once selected an option we will automatically save it to the active template (GRID OF BUTTONS)
            // when we are ready we can press Preview ->(Note the loaded template will automatically be created)
            // Note that while editing the player camera will not be interacting (no movement no camera rotation).
            // Once we have a template loaded or changed we can test (Gui will be hidden) except a button exit test which will enable the gui back and lock player again.

            //GridManager.Instance.CreatePredefinedGrid(GetActiveTemplateMapping(), false); //Create a grid composite of the elements
        }
    }

    //Member Functions
    public List<UserDefinedMappedDifficultySO> GetAllTemplates() => userDefinedMappedDifficultySOs;

    public  void LoadTemplate(int index) => SetActiveTemplate(CorrectIndex(index));

    public void SaveChangesToTemplate(){
        GameObject[,] latestUpdatedMapping = _activeMappedDifficultyContainer.GetActiveTemplateHolderMapping();
        userDefinedMappedDifficultySOs[activeTemplateIndex].SaveTemplateMapping(latestUpdatedMapping);
    }

    public void UpdateActiveTemplate(GameObject[,] tileMapping) => _activeMappedDifficultyContainer.ModifyMapping(tileMapping);

    public GameObject[,] GetActiveTemplateMapping() => _activeMappedDifficultyContainer.GetActiveTemplateHolderMapping();

    private int CorrectIndex(int index) {
        if (index >= userDefinedMappedDifficultySOs.Count) index = 0;
        else if (index < 0) index = userDefinedMappedDifficultySOs.Count - 1;
        return index;
    }

    private void SetActiveTemplate(int index){
        activeTemplateIndex = CorrectIndex(index);
        _activeMappedDifficultyContainer.ModifyMapping(userDefinedMappedDifficultySOs[activeTemplateIndex].GetConvertedTileMapToGameObjects());
    }
}