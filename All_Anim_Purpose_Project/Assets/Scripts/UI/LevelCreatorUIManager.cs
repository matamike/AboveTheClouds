using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCreatorUIManager : Singleton<LevelCreatorUIManager> {
    [SerializeField] GridLayoutGroup tileGroupGridLayout;
    [SerializeField] VerticalLayoutGroup templateGroupVerticalLayout;
    [SerializeField] GameObject tileUIPrefabButton,templateUIPrefabButton;
    [SerializeField] GameObject toggleButton, toggleEntity;
    [SerializeField] Vector2Int targetGridSize;
    private List<UserDefinedMappedDifficultySO> _userDifficultySOs = new List<UserDefinedMappedDifficultySO>();
    private bool _hasInitialized = false;
    private RectTransform tileGroupGridLayoutRectTransform;

    private void Start(){
        InitializeToggle();
    }

    private void LateUpdate(){
        if (_hasInitialized && toggleEntity.activeInHierarchy) ComputeGridCellSize(targetGridSize.x, targetGridSize.y);
    }

    private void InitializeToggle(){
        toggleButton.GetComponent<Button>().onClick.AddListener(() =>{ ToggleUI();});
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

    private void ToggleUI(){
        toggleEntity.SetActive(!toggleEntity.activeInHierarchy);
        if (!_hasInitialized && toggleEntity.activeInHierarchy){
            _hasInitialized = true;
            tileGroupGridLayoutRectTransform = tileGroupGridLayout.GetComponent<RectTransform>();
            PopulateUserDefinedDifficultySOsList();
            CreateTemplateList();
        }
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
                SetUIGridSize(grid.GetLength(0), grid.GetLength(1));
                CreateUIGrid(index);
            });
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
        uiGridTile.SetTileType(tileType);

        //Listener for each Grid Tile Button
        item.GetComponent<Button>().onClick.AddListener(() =>{
            TileTypeUIManager.Instance.EnableTileTypePanel();
            TileTypeUIManager.Instance.SetSelectedGridTile(uiGridTile);
        });
    }

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
}