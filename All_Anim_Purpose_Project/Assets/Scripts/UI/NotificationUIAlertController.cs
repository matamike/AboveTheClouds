using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUIAlertController : Singleton<NotificationUIAlertController>{
    [SerializeField] private GameObject notificationAlertCanvasGO;
    [SerializeField] private Button acceptButton, declineButton;
    [SerializeField] private TextMeshProUGUI alertText;
    private Action acceptCallback, declineCallback;

    private void Start(){
        InitializeButtonCallbacks();
        ToggleAlertNotificationUI(false);
    }

    public void SetAlertMessage(string message) => alertText.text = message;
    public void UpdateAcceptButtonDisplayText(string displayText)=> acceptButton.GetComponentInChildren<TextMeshProUGUI>().text = displayText;
    public void UpdateDeclineButtonDisplayText(string displayText) => declineButton.GetComponentInChildren<TextMeshProUGUI>().text = displayText;

    public void SetCallbackActionContinueWithSave(Action action)=> acceptCallback = action;
    public void SetCallbackActionContinueWithoutSave(Action action) => declineCallback = action;
    public void ToggleAlertNotificationUI(bool flag) => notificationAlertCanvasGO.SetActive(flag);
    private void InitializeButtonCallbacks(){
        acceptButton.onClick.AddListener(() => {
            if (acceptCallback != null){
                acceptCallback();
                acceptCallback = null;
            }
            ToggleAlertNotificationUI(false);
        });

        declineButton.onClick.AddListener(() =>{
            if (declineCallback != null){
                declineCallback();
                declineCallback = null;
            }
            ToggleAlertNotificationUI(false);
        });
    }
}
