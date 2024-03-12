using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUIManager : Singleton<OptionsUIManager>{
    [SerializeField] private Button togglePreferencesUIButton; //toggle button
    [SerializeField] private Button quitGameButton;
    [SerializeField] private GameObject preferencesPanelContainer; //main container(all gui)
    [SerializeField] private GameObject uxToggleSettingPrefab; //access to toggle (on/off) type of setting template
    [SerializeField] private GameObject uxSettingsContainer; //settings instances container(vertical layout group)
    [SerializeField] private TMP_Dropdown resolutionDropdown; //will be the holder for changing resolution. 

    private readonly Dictionary<int, Vector2Int> resolutions = new Dictionary<int, Vector2Int>(){
        { 0, new Vector2Int(1024, 768) },
        { 1, new Vector2Int(1280, 720) },
        { 2, new Vector2Int(1280, 1024)},
        { 3, new Vector2Int(1366, 768) },
        { 4, new Vector2Int(1920, 1080) },
        { 5, new Vector2Int(2560, 1440) },
        { 6, new Vector2Int(3840, 2160) },
    };


    private void Start(){
        InitializePreferencesUI();        
    }

    private void OnEnable(){
        IUICursorToggle.OnToggle += Cursor_OnToggle;
        IUICursorToggle.OnForceClose += Cursor_OnForceClose;
    }
    private void OnDisable(){
        IUICursorToggle.OnToggle -= Cursor_OnToggle;
        IUICursorToggle.OnForceClose -= Cursor_OnForceClose;
    }

    private void Cursor_OnToggle(object sender, EventArgs args){
        if (sender.GetType() == typeof(OptionsUIManager)){
            TogglePreferencesUI();
        }
    }

    private void Cursor_OnForceClose(object sender, EventArgs args) => CloseUI();

    private void InitializePreferencesUI(){
        //Toggle Button Setup
        togglePreferencesUIButton.onClick.RemoveAllListeners();
        togglePreferencesUIButton.onClick.AddListener(() => {
            TogglePreferencesUI();
        });

        //Quit Game Button Setup
        quitGameButton.onClick.RemoveAllListeners();
        quitGameButton.onClick.AddListener(() => {
            Application.Quit();
        });

        //UX Setting Setup
        GameObject uxSetting = Instantiate(uxToggleSettingPrefab, uxSettingsContainer.transform);
        TextMeshProUGUI uxSettingText = uxSetting.GetComponent<TextMeshProUGUI>();
        uxSettingText.text = PreferencesUtility.GetUXActivationKey();
        Button onButton = uxSetting.transform.Find("On").GetComponent<Button>();
        Button offButton = uxSetting.transform.Find("Off").GetComponent<Button>();
        onButton.onClick.RemoveAllListeners();
        offButton.onClick.RemoveAllListeners();

        onButton.onClick.AddListener(() =>{
            PreferencesUtility.ToggleUX(PreferencesUtility.GetUXActivationKey(), true);
        });

        offButton.onClick.AddListener(() =>{
            PreferencesUtility.ToggleUX(PreferencesUtility.GetUXActivationKey(), false);
        });

        bool valuehasBeenSet = false;
        foreach(var item in resolutions){
            if(Screen.currentResolution.width == item.Value.x && Screen.currentResolution.height == item.Value.y){
                resolutionDropdown.value = item.Key;
                valuehasBeenSet = true;
            }
        }

        if (!valuehasBeenSet) resolutionDropdown.value = -1;

        //Dropdown Resolution
        resolutionDropdown.onValueChanged.AddListener((int id) => {
            if(resolutions.ContainsKey(id)){
                Debug.Log("Selected Resolution: " + resolutions[id]);
                Screen.SetResolution(resolutions[id].x, resolutions[id].y, true);
            } 
        });
    }

    private void TogglePreferencesUI(){
        preferencesPanelContainer.SetActive(!preferencesPanelContainer.activeInHierarchy);
        if (!preferencesPanelContainer.activeInHierarchy){
            CursorVisibilityUtility.ForceCloseAllEntities(this);
        }
    }

    private void CloseUI() => preferencesPanelContainer.SetActive(false);
}
