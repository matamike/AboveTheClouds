using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHighlightShadow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    private Shadow _shadow;
    private RectTransform _rectTransform;
    private bool _isHovering = false;

    private void Awake(){
        _shadow = GetComponent<Shadow>();
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable(){
        TileTypeUIManager.Instance.OnTileTypeSelected += TileTypeUIManager_OnTileTypeSelected;
        TileTypeUIManager.Instance.OnToggleTileTypeSelectionGUI += TileTypeUIManager_OnToggleTileTypeSelectionGUI;
    }

    private void OnDisable(){
        TileTypeUIManager.Instance.OnTileTypeSelected -= TileTypeUIManager_OnTileTypeSelected;
        TileTypeUIManager.Instance.OnToggleTileTypeSelectionGUI -= TileTypeUIManager_OnToggleTileTypeSelectionGUI;
    }

    private void TileTypeUIManager_OnToggleTileTypeSelectionGUI(object sender, TileTypeUIManager.OnToggleTileTypeSelectionGUIEventArgs e){
        if (!e.toggle) FocusSelection(false);
    }

    private void TileTypeUIManager_OnTileTypeSelected(object sender, TileTypeUIManager.OnTileTypeSelectedEventArgs e){
        if(e.selectedButton.gameObject == gameObject) FocusSelection(true);
        else FocusSelection(false);
    }

    public void OnPointerEnter(PointerEventData eventData){
        _shadow.enabled = true;
        _isHovering = true;
    }
    public void OnPointerExit(PointerEventData eventData){
        _shadow.enabled = false;
        _isHovering = false;
    }
    public void FocusSelection(bool flag) => _rectTransform.localScale = (flag && _isHovering) ? Vector3.one * 0.75f : Vector3.one;
}