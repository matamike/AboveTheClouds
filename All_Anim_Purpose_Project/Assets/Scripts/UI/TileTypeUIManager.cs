using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileTypeUIManager : Singleton<TileTypeUIManager>{
    public event EventHandler<OnTileTypeSelectedEventArgs> OnTileTypeSelected;
    public event EventHandler<OnToggleTileTypeSelectionGUIEventArgs> OnToggleTileTypeSelectionGUI;

    public class OnTileTypeSelectedEventArgs: EventArgs{
        public Button selectedButton;
    }

    public class OnToggleTileTypeSelectionGUIEventArgs: EventArgs{
        public bool toggle;
    }

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
            
            //external refs passed to on click listener for each tile type button.
            TileType.Type typeToSetWith = entry.Key;
            Button button = buttonGO.GetComponent<Button>();

            //Listeners to button GO
            button.onClick.AddListener(() => {
                if (selectedGridTile is not null){
                    selectedGridTile.SetUnconfirmedTileType(typeToSetWith);
                    OnTileTypeSelected?.Invoke(this, new OnTileTypeSelectedEventArgs { selectedButton = button });
                }
            });
        }

        //Close Button Listener
        _closeButton.GetComponent<Button>().onClick.AddListener(() =>{
            OnToggleTileTypeSelectionGUI?.Invoke(this, new OnToggleTileTypeSelectionGUIEventArgs { toggle = false });
            selectedGridTile.ResetUnconfirmedTileType(); //clear any change (clean state in next iterations)
            selectedGridTile = null;
            DisableTileTypePanel();         
        });

        //Confirm Button Listener
        _confirmButton.GetComponent<Button>().onClick.AddListener(() => {
            OnToggleTileTypeSelectionGUI?.Invoke(this, new OnToggleTileTypeSelectionGUIEventArgs { toggle = false });
            //Confirm latest option changed for selected tile
            selectedGridTile.ConfirmTileType();
            //Change Color for the grid Tile
            selectedGridTile.gameObject.GetComponent<Image>().color = TileTypeUtility.GetTypeColor(selectedGridTile.GetTileType());
            //Change Display Text for the grid Tile
            Vector2Int indices = selectedGridTile.GetIndices();
            string displayButtonText = selectedGridTile.GetTileType().ToString() + "\n" +"[" + indices.x + "," + indices.y + "]";
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