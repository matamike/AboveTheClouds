using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCreatorUIManager : Singleton<LevelCreatorUIManager> {
    public event EventHandler<OnTemplateChangedEventArgs> OnTemplateChanged;
    public event EventHandler<OnTemplateRequestSaveArgs> OnTemplateRequestSave;

    public class OnTemplateRequestSaveArgs : EventArgs {
        public List<Tuple<Vector2Int, TileType.Type>> changes;
    }

    public class OnTemplateChangedEventArgs : EventArgs{
        public int templateIndex;
    }

    [SerializeField] private GridLayoutGroup tileGroupGridLayout;
    [SerializeField] private VerticalLayoutGroup templateGroupVerticalLayout;
    [SerializeField] private GameObject tileUIPrefabButton,templateUIPrefabButton;
    [SerializeField] private GameObject toggleButton, toggleEntity;
    [SerializeField] private GameObject saveChangesButton;
    [SerializeField] private GameObject testMapButton;
    [SerializeField] private Canvas levelCreatorCanvas;
    private Vector2Int targetGridSize;
    private List<UserDefinedMappedDifficultySO> _userDifficultySOs = new List<UserDefinedMappedDifficultySO>();
    private bool _hasInitialized = false;
    private RectTransform tileGroupGridLayoutRectTransform;

    // Variables
    private List<Tuple<Vector2Int, TileType.Type>> _unsavedChanges = new List<Tuple<Vector2Int, TileType.Type>>();

    private void OnEnable(){
        IUICursorToggle.OnToggle += Cursor_OnToggle;
        IUICursorToggle.OnForceClose += Cursor_OnForceClose;
    }
    private void OnDisable(){
        IUICursorToggle.OnToggle -= Cursor_OnToggle;
        IUICursorToggle.OnForceClose -= Cursor_OnForceClose;
    }

    private void Start(){
        InitializeToggle();
        InitializeTemplates();
    }

    private void LateUpdate(){
        if (_hasInitialized && toggleEntity.activeInHierarchy) ComputeGridCellSize(targetGridSize.x, targetGridSize.y);
    }

    private void Cursor_OnToggle(object sender, EventArgs args){
        if (sender.GetType() == typeof(LevelCreatorUIManager)) ToggleUI();
    }

    private void Cursor_OnForceClose(object sender, EventArgs args) => CloseUI();

    private void InitializeToggle(){
        toggleButton.GetComponent<Button>().onClick.AddListener(() =>{ ToggleUI();});
    }

    private void InitializeTemplates(){
        _hasInitialized = true;
        tileGroupGridLayoutRectTransform = tileGroupGridLayout.GetComponent<RectTransform>();
        PopulateUserDefinedDifficultySOsList();
        CreateTemplateList();

        //Save Button Listener
        saveChangesButton.GetComponent<Button>().onClick.AddListener(() => {
            if (_unsavedChanges.Count > 0) SaveProcess();
        });

        //Test Map Button Listener
        testMapButton.GetComponent<Button>().onClick.AddListener(() => {
            //AutoSave
            if (_unsavedChanges.Count > 0) SaveProcess();
            //Hide Canvas
            ToggleCanvas(false);
            //Set Callback On Exit from Test
            TestMapUIManager.Instance.SetExitTestAction(() => { ToggleCanvas(true); CursorVisibilityUtility.SetCursorVisibility(true); });
            TestMapUIManager.Instance.EnableTestUI();
            TestMapUIManager.Instance.CreateTestLevel();

            CursorVisibilityUtility.SetCursorVisibility(false);
        });    
    }

    private void PopulateUserDefinedDifficultySOsList(){
        _userDifficultySOs = UserDefinedDifficultyTemplatesController.Instance.GetAllTemplates();
    }

    private void SetUIGridSize(int x, int y){
        if (targetGridSize.x != x || targetGridSize.y != y){
            targetGridSize = new Vector2Int(x, y);
            ComputeGridCellSize(x, y);
        }
    }

    public void ToggleCanvas(bool toggle) => levelCreatorCanvas.enabled = toggle;

    private void CloseUI(){
        toggleEntity.SetActive(false);
        ToggleTestMapButton(false);
    }

    private void ToggleUI(){
        if (levelCreatorCanvas.enabled){
            if (_hasInitialized && toggleEntity.activeInHierarchy){
                bool hasChanges = CheckPendingChanges();
                if (hasChanges) return;
            }

            //Toggle
            toggleEntity.SetActive(!toggleEntity.activeInHierarchy);

            //Disable TestMap Button when Toggling the main Level Creator GUI (ON)
            if (toggleEntity.activeInHierarchy) ToggleTestMapButton(false);

            //Force Close All Interacting Entities
            if (!toggleEntity.activeInHierarchy) CursorVisibilityUtility.ForceCloseAllEntities(this);
        }
        else{
            CursorVisibilityUtility.SetCursorVisibility(false);
        }
    }

    private bool CheckPendingChanges(){
        if (HasPendingChanges()){
            //Set Callback for ContinueWithSave Button
            NotificationUIAlertController.Instance.SetCallbackActionContinueWithSave(() => {
                OnTemplateRequestSave?.Invoke(this, new OnTemplateRequestSaveArgs { changes = _unsavedChanges });
                ResetGUIElementsToDefaultState();
                toggleEntity.SetActive(!toggleEntity.activeInHierarchy);
            });

            //Set Callback for ContinueWithoutSave Button
            NotificationUIAlertController.Instance.SetCallbackActionContinueWithoutSave(() => {
                ResetGUIElementsToDefaultState();
                toggleEntity.SetActive(!toggleEntity.activeInHierarchy);
            });

            //Enable Notification Alert
            NotificationUIAlertController.Instance.ToggleAlertNotificationUI(true);
            return true;
        }
        else{
            ResetGUIElementsToDefaultState();
            return false;
        }
    }

    private void SaveProcess(){
        OnTemplateRequestSave?.Invoke(this, new OnTemplateRequestSaveArgs { changes = _unsavedChanges });
        ToggleSaveChangesButton(false);
        ClearChanges();
    }

    private void CreateTemplateList(){
        ClearChilds(templateGroupVerticalLayout.gameObject);
        //Create 
        for (int x = 0; x < _userDifficultySOs.Count; x++){
            string name = "Template " + _userDifficultySOs[x].name.ToString();
            GameObject[,] grid = _userDifficultySOs[x].GetConvertedTileMapToGameObjects();
            GameObject button = CreateTemplateListButton(name, grid.GetLength(0), grid.GetLength(1));

            //Template Button Listener (Switch Grid and Template Activator)
            int index = x;
            button.GetComponent<Button>().onClick.AddListener(() =>{
                if (_unsavedChanges.Count > 0){
                    NotificationUIAlertController.Instance.SetCallbackActionContinueWithoutSave(() =>{
                        TemplateChange(index, grid);
                        ToggleSaveChangesButton(false);
                        ClearChanges();
                    });

                    NotificationUIAlertController.Instance.SetCallbackActionContinueWithSave(() =>{
                        OnTemplateRequestSave?.Invoke(this, new OnTemplateRequestSaveArgs { changes = _unsavedChanges });
                        TemplateChange(index, grid);
                        ToggleSaveChangesButton(false);
                        ClearChanges();
                    });
                    NotificationUIAlertController.Instance.ToggleAlertNotificationUI(true);
                }
                else{
                    TemplateChange(index, grid);
                }
                ToggleTestMapButton(true);
            });
        }
    }

    private void TemplateChange(int index, GameObject[,] grid){
        OnTemplateChanged?.Invoke(this, new OnTemplateChangedEventArgs { templateIndex = index });
        SetUIGridSize(grid.GetLength(0), grid.GetLength(1));
        CreateUIGrid(index);
    }

    private void ResetGUIElementsToDefaultState(){
        ToggleSaveChangesButton(false);
        ClearChanges();
        TileTypeUIManager.Instance.DisableTileTypePanel();
        ClearChilds(tileGroupGridLayout.gameObject);
        
    }

    private void CreateUIGrid(int index){
        ClearChilds(tileGroupGridLayout.gameObject);
        ComputeGridCellSize(targetGridSize.x, targetGridSize.y);
        //Create 
        for (int x = 0; x < targetGridSize.x; x++){
            for (int y = 0; y < targetGridSize.y; y++){
                TileType.Type tileType = _userDifficultySOs[index].GetGridTileMapValue(x, y);
                Color color = _userDifficultySOs[index].GetGridTileMapColor(x, y);
                CreateTileGridButton(x, y, tileType, color);
            }
        }
    }

    private void ClearChilds(GameObject parent){
        int childCount = parent.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--){
            Transform child = tileGroupGridLayout.gameObject.transform.GetChild(i);
            if (child != null) Destroy(child.gameObject);
        } 
    }

    private void CreateTileGridButton(int x, int y, TileType.Type tileType, Color tileTypeColor){
        GameObject item = Instantiate(tileUIPrefabButton, tileGroupGridLayout.transform);
        item.name = "[" + x + "," + y + "]";
        item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = tileType.ToString() + "\n" + item.name;
        item.GetComponent<Image>().color = tileTypeColor;
        UIGridTile uiGridTile = item.GetComponent<UIGridTile>();
        uiGridTile.SetIndices(x, y);
        uiGridTile.SetInitialTileType(tileType);

        //Listener for each Grid Tile Button
        item.GetComponent<Button>().onClick.AddListener(() =>{
            TileTypeUIManager.Instance.EnableTileTypePanel();
            //todo clear temp save (todo throw an alert before attempting to close) and prompt user to decide whether to save or not (same thing needs to happen in
            //template change
            TileTypeUIManager.Instance.SetSelectedGridTile(uiGridTile);
        });
    }

    public void ToggleSaveChangesButton(bool toggle) => saveChangesButton.SetActive(toggle);

    public void ToggleTestMapButton(bool toggle) => testMapButton.SetActive(toggle);

    private GameObject CreateTemplateListButton(string name, int xCapacity,  int yCapacity){
        GameObject item = Instantiate(templateUIPrefabButton, templateGroupVerticalLayout.transform);
        item.name = name;
        item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.name + "\n" + "," + "Capacity["+xCapacity + "," + yCapacity + "]";
        return item;
    }

    private void ComputeGridCellSize(int targetGridSizeX, int targetGridSizeY){
        if (targetGridSizeX <= 0 || targetGridSizeY <= 0) return;

        //Current Rect size
        float width = tileGroupGridLayoutRectTransform.rect.width;
        float height = tileGroupGridLayoutRectTransform.rect.height;
        
        //Target new Cell Size
        Vector2 newCellSize;
        if (targetGridSizeX == targetGridSizeY) newCellSize = new Vector2(width / targetGridSizeX, height / targetGridSizeY);
        else newCellSize = new Vector2(width / targetGridSizeY, height / targetGridSizeX);

        //Assign new Cell Size
        tileGroupGridLayout.cellSize = newCellSize;
    }
    
    //Change based functions
    public bool HasPendingChanges() => (_unsavedChanges.Count > 0) ? true : false;
    public void AddPendingSaveChange(Tuple<Vector2Int, TileType.Type> change){
        //Check if we already have a change in unsaved to alter it.
        for(int i=0; i<_unsavedChanges.Count; i++){
            if (_unsavedChanges[i].Item1 == change.Item1){
                _unsavedChanges[i] = change;
                return;
            }
        }
        //brand new change
        _unsavedChanges.Add(change);
    }
    public void ClearChanges() => _unsavedChanges.Clear();
    public List<Tuple<Vector2Int, TileType.Type>> GetChanges() => _unsavedChanges;
}