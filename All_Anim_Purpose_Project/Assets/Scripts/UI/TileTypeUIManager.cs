using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileTypeUIManager : Singleton<TileTypeUIManager>{
    [SerializeField] GameObject _tileTypeButtonPanel;
    [SerializeField] GameObject _tileTypeButtonContainer;
    [SerializeField] GameObject _tileTypeButtonPrefab;
    [SerializeField] GameObject _confirmButton, _closeButton;
    private UIGridTile selectedGridTile;

    private void Start(){
        DisableTileTypePanel();
        InitializeComponents();
    }
    
    private void InitializeComponents(){
        //Initialize buttonContainer
        Dictionary<TileType.Type, Color> dict = TileTypeUtility.GetTileTypeDictionary();
        foreach (var entry in dict) {
            GameObject buttonGO = Instantiate(_tileTypeButtonPrefab, _tileTypeButtonContainer.transform);
            buttonGO.name = entry.Key.ToString();
            buttonGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = entry.Key.ToString();
            buttonGO.GetComponent<Image>().color = entry.Value;
            //Listeners to buttonGO
            TileType.Type typeToSetWith = entry.Key;
            Button button = buttonGO.GetComponent<Button>();

            //OnClick
            button.onClick.AddListener(() => {
                if (selectedGridTile is not null){
                    selectedGridTile.SetTileType(typeToSetWith);
                }
            });
        }

        _closeButton.GetComponent<Button>().onClick.AddListener(() =>{
            DisableTileTypePanel();
            selectedGridTile = null;
        });

        _confirmButton.GetComponent<Button>().onClick.AddListener(() => {
            //Change Color for the grid Tile
            selectedGridTile.gameObject.GetComponent<Image>().color = TileTypeUtility.GetTypeColor(selectedGridTile.GetTileType());
            //Change Display Text for the grid Tile
            Vector2Int indices = selectedGridTile.GetIndices();
            string displayButtonText = selectedGridTile.GetTileType().ToString() + "\n" +"[" + indices.x + ", " + indices.y + "]";
            selectedGridTile.gameObject.GetComponent<Button>().transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = displayButtonText;
            //Lastly Close the panel and remove the selected
            DisableTileTypePanel();
            selectedGridTile = null;
        });
    }
    public void SetSelectedGridTile(UIGridTile uIGridTile) => selectedGridTile = uIGridTile;
    public void EnableTileTypePanel() => _tileTypeButtonPanel.SetActive(true);
    public void DisableTileTypePanel() => _tileTypeButtonPanel.SetActive(false);
}