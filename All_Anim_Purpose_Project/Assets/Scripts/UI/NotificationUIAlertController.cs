using System;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUIAlertController : Singleton<NotificationUIAlertController>{
    [SerializeField] private GameObject notificationAlertCanvasGO;
    [SerializeField] private Button continueWithSaveButton, continueWithoutSaveButton;
    private Action continueWithSaveCallback, continueWithoutSaveCallback;

    private void Start(){
        InitializeButtonCallbacks();
        ToggleAlertNotificationUI(false);
    }

    public void SetCallbackActionContinueWithSave(Action action){
        Debug.Log("Setting Callback (ContinueWithSave)");
        continueWithSaveCallback = action;
    }
    public void SetCallbackActionContinueWithoutSave(Action action){
        Debug.Log("Setting Callback (ContinueWithoutSave)");
        continueWithoutSaveCallback = action;
    }
    public void ToggleAlertNotificationUI(bool flag){
        Debug.Log("Canvas Enabled State -> " + flag.ToString());
        notificationAlertCanvasGO.SetActive(flag);
    }
    private void InitializeButtonCallbacks(){
        continueWithSaveButton.onClick.AddListener(() => {
            if (continueWithSaveCallback != null){
                continueWithSaveCallback();
                continueWithSaveCallback = null;
            }
            ToggleAlertNotificationUI(false);
        });

        continueWithoutSaveButton.onClick.AddListener(() =>{
            if (continueWithoutSaveCallback != null){
                continueWithoutSaveCallback();
                continueWithoutSaveCallback = null;
            }
            ToggleAlertNotificationUI(false);
        });
    }
}
