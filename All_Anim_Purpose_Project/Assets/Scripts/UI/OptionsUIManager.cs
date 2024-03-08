using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUIManager : Singleton<OptionsUIManager>{
    [SerializeField] private Button togglePreferencesUIButton; //toggle button
    [SerializeField] private Button quitGameButton;
    [SerializeField] private GameObject preferencesPanelContainer; //main container(all gui)
    [SerializeField] private GameObject uxToggleSettingPrefab; //access to toggle (on/off) type of setting template
    [SerializeField] private GameObject uxSettingsContainer; //settings instances container(vertical layout group)

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
    }

    private void TogglePreferencesUI(){
        preferencesPanelContainer.SetActive(!preferencesPanelContainer.activeInHierarchy);
        if (!preferencesPanelContainer.activeInHierarchy){
            CursorVisibilityUtility.ForceCloseAllEntities(this);
        }
    }

    private void CloseUI() => preferencesPanelContainer.SetActive(false);
}
