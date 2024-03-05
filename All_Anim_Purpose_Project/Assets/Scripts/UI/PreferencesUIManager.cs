using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreferencesUIManager : Singleton<PreferencesUIManager>{
    [SerializeField] private Button togglePreferencesUIButton; //toggle button
    [SerializeField] private GameObject preferencesPanelContainer; //main container(all gui)
    [SerializeField] private GameObject uxToggleSettingPrefab; //access to toggle (on/off) type of setting template
    [SerializeField] private GameObject uxSettingsContainer; //settings instances container(vertical layout group)

    private void Start(){
        InitializePreferencesUI();        
    }

    private void OnCursorShow(object sender, EventArgs args) {
        Debug.Log("Show Cursor");
        Debug.Log("Disable UI if open");
    }

    private void OnCursorHide(object sender, EventArgs args){
        Debug.Log("Hide Cursor");
        Debug.Log("Enable UI if closed");
    }

    private void OnEnable(){
        IUICursorToggle.OnShow += OnCursorShow;
        IUICursorToggle.OnHide += OnCursorHide;      
    }

    private void OnDisable(){
        IUICursorToggle.OnShow -= OnCursorShow;
        IUICursorToggle.OnHide -= OnCursorHide;
    }

    //        TogglePreferencesUI();
    //        CursorVisibilityUtility.SetCursorVisibility(preferencesPanelContainer.activeInHierarchy);
    //        InputManager.Instance.SetControlLockStatus(preferencesPanelContainer.activeInHierarchy);
    //        CameraController.Instance.SetLockCameraStatus(preferencesPanelContainer.activeInHierarchy);

    private void InitializePreferencesUI(){
        //Toggle Button Setup
        togglePreferencesUIButton.onClick.RemoveAllListeners();
        togglePreferencesUIButton.onClick.AddListener(() => {
            TogglePreferencesUI();
            //CursorVisibilityUtility.SetCursorVisibility(preferencesPanelContainer.activeInHierarchy);
            //InputManager.Instance.SetControlLockStatus(preferencesPanelContainer.activeInHierarchy);
            //CameraController.Instance.SetLockCameraStatus(preferencesPanelContainer.activeInHierarchy);
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

   private void TogglePreferencesUI() => preferencesPanelContainer.SetActive(!preferencesPanelContainer.activeInHierarchy);
}
