using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserDefinedTemplateUIController : Singleton<UserDefinedTemplateUIController>{
    public event EventHandler<OnCustomDifficultySelectedEventArgs> OnCustomDifficultySelected;

    public class OnCustomDifficultySelectedEventArgs : EventArgs{
        public UserDefinedMappedDifficultySO userDefinedMappedDifficultySO;
    }

    [SerializeField] private GridLayoutGroup tileGroupGridLayout;
    [SerializeField] private VerticalLayoutGroup templateGroupVerticalLayout;
    [SerializeField] private GameObject mappingTextMeshPreview, templateUIPrefabButton;
    [SerializeField] private GameObject toggleButton, container;
    [SerializeField] private GameObject confirmSelectionButton;
    [SerializeField] private Canvas userDefinedTemplateUICanvas;
    private Vector2Int targetGridSize;
    private List<UserDefinedMappedDifficultySO> _userDifficultySOs = new List<UserDefinedMappedDifficultySO>();
    private bool _hasInitialized = false;
    private RectTransform tileGroupGridLayoutRectTransform;

    private void Start(){
        InitializeToggle();
        ToggleCanvas();
    }

    private void LateUpdate(){
        if (_hasInitialized && container.activeInHierarchy) ComputeGridCellSize(targetGridSize.x, targetGridSize.y);
    }

    private void InitializeToggle(){
        toggleButton.GetComponent<Button>().onClick.AddListener(() => { ToggleUI(); });
    }

    public void ToggleCanvas() => userDefinedTemplateUICanvas.enabled = !userDefinedTemplateUICanvas.enabled;

    private void ToggleUI(){
        //True->False UserDefinedDifficulty UI
        if (_hasInitialized && container.activeInHierarchy){
            ResetGUIElementsToDefaultState();
            container.SetActive(!container.activeInHierarchy);
            return;
        }

        //False->True Levelcreator GUI
        container.SetActive(!container.activeInHierarchy);


        //1st time initilization (save templates setup)
        if (!_hasInitialized && container.activeInHierarchy)
        {
            _hasInitialized = true;
            tileGroupGridLayoutRectTransform = tileGroupGridLayout.GetComponent<RectTransform>();
            PopulateUserDefinedDifficultySOsList();
            CreateTemplateList();
        }

        //Disable Confirm Selection
        if(!container.activeInHierarchy) ToggleConfirmSelectionButton(true);
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
            button.GetComponent<Button>().onClick.AddListener(() => {
                TemplateChange(index, grid);
                ToggleConfirmSelectionButton(true);
                SetConfirmButtonCallback(index);
            });
        }
    }

    private void ToggleConfirmSelectionButton(bool state) => confirmSelectionButton.SetActive(state);

    private void SetConfirmButtonCallback(int index){
        confirmSelectionButton.GetComponent<Button>().onClick.RemoveAllListeners();
        confirmSelectionButton.GetComponent<Button>().onClick.AddListener(() =>{
            List<UserDefinedMappedDifficultySO> customDiffSOsList =  UserDefinedDifficultyTemplatesController.Instance.GetAllTemplates();
            OnCustomDifficultySelected?.Invoke(this, new OnCustomDifficultySelectedEventArgs {
                userDefinedMappedDifficultySO = customDiffSOsList[index]
            });
            ToggleUI();
            ToggleCanvas();
            ToggleConfirmSelectionButton(false);
        });
    }

    private void SetUIGridSize(int x, int y){
        if (targetGridSize.x != x || targetGridSize.y != y)
        {
            targetGridSize = new Vector2Int(x, y);
            ComputeGridCellSize(x, y);
        }
    }

    private void CreateUIGrid(int index){
        ClearChilds(tileGroupGridLayout.gameObject);
        ComputeGridCellSize(targetGridSize.x, targetGridSize.y);
        //Create 
        for (int x = 0; x < targetGridSize.x; x++){
            for (int y = 0; y < targetGridSize.y; y++){
                TileType.Type tileType = _userDifficultySOs[index].GetGridTileMapValue(x, y);
                Color color = _userDifficultySOs[index].GetGridTileMapColor(x, y);
                CreateTileGridPreview(x, y, tileType, color);
            }
        }
    }

    private void CreateTileGridPreview(int x, int y, TileType.Type tileType, Color tileTypeColor){
        GameObject item = Instantiate(mappingTextMeshPreview, tileGroupGridLayout.transform);
        item.name = "[" + x + "," + y + "]";
        item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = tileType.ToString() + "\n" + item.name;
        item.GetComponent<Image>().color = tileTypeColor;
        UIGridTile uiGridTile = item.GetComponent<UIGridTile>();
        uiGridTile.SetIndices(x, y);
        uiGridTile.SetInitialTileType(tileType);
    }

    private void TemplateChange(int index, GameObject[,] grid){
        SetUIGridSize(grid.GetLength(0), grid.GetLength(1));
        CreateUIGrid(index);
    }

    private GameObject CreateTemplateListButton(string name, int xCapacity, int yCapacity){
        GameObject item = Instantiate(templateUIPrefabButton, templateGroupVerticalLayout.transform);
        item.name = name;
        item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.name + "\n" + "," + "Capacity[" + xCapacity + "," + yCapacity + "]";
        return item;
    }

    private void ResetGUIElementsToDefaultState() => ClearChilds(tileGroupGridLayout.gameObject);

    private void ClearChilds(GameObject parent){
        int childCount = parent.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--){
            Transform child = tileGroupGridLayout.gameObject.transform.GetChild(i);
            if (child != null) Destroy(child.gameObject);
        }
    }

    private void PopulateUserDefinedDifficultySOsList(){
        _userDifficultySOs = UserDefinedDifficultyTemplatesController.Instance.GetAllTemplates();
    }

    private void ComputeGridCellSize(int targetGridSizeX, int targetGridSizeY)
    {
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
}
